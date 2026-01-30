using Rtl.Core.Domain.Events;

namespace Rtl.Module.SampleSales.Domain.Catalogs.Events;

public sealed record CatalogUpdatedDomainEvent(Guid CatalogId) : DomainEvent;
