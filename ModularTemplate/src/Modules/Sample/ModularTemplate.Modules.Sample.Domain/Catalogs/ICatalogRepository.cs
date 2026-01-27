using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Sample.Domain.Catalogs;

public interface ICatalogRepository : IRepository<Catalog, Guid>
{
}
