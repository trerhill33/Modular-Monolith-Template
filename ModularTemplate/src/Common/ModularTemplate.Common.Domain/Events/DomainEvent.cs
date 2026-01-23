namespace ModularTemplate.Common.Domain.Events;

/// <summary>
/// Base class for domain events with automatic ID and timestamp generation.
/// </summary>
/// <remarks>
/// Note: DateTime.UtcNow is used directly here because domain events are instantiated
/// at the point of raising (e.g., via <c>new SomeDomainEvent()</c>), making dependency
/// injection of IDateTimeProvider impractical. The timestamp represents when the event
/// actually occurred in the domain, which should be captured at creation time.
/// For testing scenarios, use the constructor overload that accepts explicit values.
/// </remarks>
public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    protected DomainEvent(Guid id, DateTime occurredOnUtc)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }
}
