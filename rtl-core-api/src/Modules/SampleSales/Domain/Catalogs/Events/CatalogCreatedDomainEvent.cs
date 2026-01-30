using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogCreatedDomainEvent(Guid CatalogId) : DomainEvent;
