using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.SampleSales.Domain.Products;

public interface IProductRepository : IRepository<Product, Guid>
{
}
