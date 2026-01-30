using Rtl.Core.Application.Messaging;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain.Results;
using Rtl.Core.Domain.ValueObjects;
using Rtl.Module.SampleOrders.Domain;
using Rtl.Module.SampleOrders.Domain.Orders;
using Rtl.Module.SampleOrders.Domain.ProductsCache;

namespace Rtl.Module.SampleOrders.Application.Orders.PlaceOrder;

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
