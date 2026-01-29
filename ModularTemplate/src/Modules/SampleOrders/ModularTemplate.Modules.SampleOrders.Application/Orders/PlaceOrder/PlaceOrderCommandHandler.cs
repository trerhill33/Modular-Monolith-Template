using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Common.Domain.ValueObjects;
using ModularTemplate.Modules.SampleOrders.Domain;
using ModularTemplate.Modules.SampleOrders.Domain.Orders;
using ModularTemplate.Modules.SampleOrders.Domain.ProductsCache;

namespace ModularTemplate.Modules.SampleOrders.Application.Orders.PlaceOrder;

internal sealed class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductCacheRepository productCacheRepository,
    IUnitOfWork<ISampleOrdersModule> unitOfWork)
    : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        // Get product from local cache (synced from Sales module)
        var product = await productCacheRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            return Result.Failure<Guid>(OrderErrors.ProductNotFound);
        }

        var orderResult = Order.Place(request.CustomerId);

        if (orderResult.IsFailure)
        {
            return Result.Failure<Guid>(orderResult.Error);
        }

        var order = orderResult.Value;

        // Create Money from product price
        var unitPriceResult = Money.Create(product.Price);
        if (unitPriceResult.IsFailure)
        {
            return Result.Failure<Guid>(unitPriceResult.Error);
        }

        var addLineResult = order.AddLine(request.ProductId, request.Quantity, unitPriceResult.Value);

        if (addLineResult.IsFailure)
        {
            return Result.Failure<Guid>(addLineResult.Error);
        }

        orderRepository.Add(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
