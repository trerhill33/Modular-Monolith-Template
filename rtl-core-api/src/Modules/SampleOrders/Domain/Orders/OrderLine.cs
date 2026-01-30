using Rtl.Core.Domain.Entities;
using Rtl.Core.Domain.ValueObjects;

namespace Rtl.Module.SampleOrders.Domain.Orders;

/// <summary>
/// Entity representing a line item within an Order aggregate.
/// Not an aggregate root - can only be accessed through the Order aggregate.
/// </summary>
public sealed class OrderLine : Entity
{
    private OrderLine()
    {
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; } = null!;

    public Money LineTotal => UnitPrice.Multiply(Quantity);

    internal static OrderLine Create(Guid orderId, Guid productId, int quantity, Money unitPrice)
    {
        return new OrderLine
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
