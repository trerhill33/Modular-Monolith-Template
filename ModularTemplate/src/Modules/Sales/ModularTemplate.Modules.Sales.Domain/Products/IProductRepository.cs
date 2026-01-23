using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Sales.Domain.Products;

public interface IProductRepository : IRepository<Product, Guid>
{
}
