using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleSales.Application.Products.GetProduct;

public sealed record GetProductQuery(Guid ProductId) : IQuery<ProductResponse>;
