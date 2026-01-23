using ModularTemplate.Common.Domain.Entities;
using ModularTemplate.Modules.Orders.Domain.Orders.Events;

namespace ModularTemplate.Modules.Orders.Domain.Orders;

public sealed class Order : SoftDeletableEntity
{
    private Order()
    {
    }

    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal TotalPrice { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime OrderedAtUtc { get; private set; }

    public static Order Place(Guid productId, int quantity, decimal unitPrice)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Quantity = quantity,
            TotalPrice = quantity * unitPrice,
            Status = OrderStatus.Pending,
            OrderedAtUtc = DateTime.UtcNow
        };

        order.Raise(new OrderPlacedDomainEvent(order.Id));

        return order;
    }

    public static void UpdateStatus(Order order, OrderStatus newStatus)
    {
        var oldStatus = order.Status;
        order.Status = newStatus;

        order.Raise(new OrderStatusChangedDomainEvent(order.Id, oldStatus, newStatus));
    }
}
