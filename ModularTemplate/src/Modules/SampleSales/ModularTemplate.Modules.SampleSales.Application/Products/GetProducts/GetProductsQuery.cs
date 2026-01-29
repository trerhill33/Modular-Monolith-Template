using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.SampleSales.Application.Products.GetProduct;

namespace ModularTemplate.Modules.SampleSales.Application.Products.GetProducts;

public sealed record GetProductsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<ProductResponse>>;
