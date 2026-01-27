using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Sample.Domain.Products;

public interface IProductRepository : IRepository<Product, Guid>
{
}
