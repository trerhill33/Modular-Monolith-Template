using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Sample.Application.Products.GetProduct;

namespace ModularTemplate.Modules.Sample.Application.Products.GetProducts;

public sealed record GetProductsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<ProductResponse>>;
