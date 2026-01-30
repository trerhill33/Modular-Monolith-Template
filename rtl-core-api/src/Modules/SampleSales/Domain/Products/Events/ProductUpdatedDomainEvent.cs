using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleSales.Domain.Products.Events;

public sealed record ProductUpdatedDomainEvent(Guid ProductId) : DomainEvent;
