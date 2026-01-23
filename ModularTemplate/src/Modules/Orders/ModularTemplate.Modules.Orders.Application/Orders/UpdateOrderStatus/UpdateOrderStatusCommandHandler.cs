using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Orders.Domain.Orders;

namespace ModularTemplate.Modules.Orders.Application.Orders.UpdateOrderStatus;

internal sealed class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateOrderStatusCommand>
{
    public async Task<Result> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(request.OrderId));
        }

        Order.UpdateStatus(order, request.NewStatus);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
