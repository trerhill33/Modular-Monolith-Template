using ModularTemplate.Common.Domain.Caching;

namespace ModularTemplate.Modules.Sample.Domain.OrdersCache;

/// <summary>
/// Cache entity representing orders from the Orders module.
/// This is a read-only copy maintained via integration events.
/// No audit fields or soft delete - cache entities are simple data copies.
/// </summary>
public sealed class OrderCache : ICacheProjection
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime OrderedAtUtc { get; set; }

    public DateTime LastSyncedAtUtc { get; set; }
}
