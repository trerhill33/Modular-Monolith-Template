using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Sales.Application.Products.GetProduct;

namespace ModularTemplate.Modules.Sales.Application.Products.GetProducts;

public sealed record GetProductsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<ProductResponse>>;
