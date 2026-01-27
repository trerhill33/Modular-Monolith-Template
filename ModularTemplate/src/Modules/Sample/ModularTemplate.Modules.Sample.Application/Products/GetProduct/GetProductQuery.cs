using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sample.Application.Products.GetProduct;

public sealed record GetProductQuery(Guid ProductId) : IQuery<ProductResponse>;
