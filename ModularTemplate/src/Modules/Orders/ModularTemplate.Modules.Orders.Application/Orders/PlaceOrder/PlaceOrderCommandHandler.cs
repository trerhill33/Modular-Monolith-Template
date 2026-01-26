using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Orders.Domain;
using ModularTemplate.Modules.Orders.Domain.Orders;
using ModularTemplate.Modules.Orders.Domain.ProductsCache;

namespace ModularTemplate.Modules.Orders.Application.Orders.PlaceOrder;

internal sealed class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductCacheRepository productCacheRepository,
    IUnitOfWork<IOrdersModule> unitOfWork)
    : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        // Get product from local cache (synced from Sales module)
        ProductCache? product = await productCacheRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            return Result.Failure<Guid>(OrderErrors.ProductNotFound);
        }

        var order = Order.Place(request.ProductId, request.Quantity, product.Price);

        orderRepository.Add(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
