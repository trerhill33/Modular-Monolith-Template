using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleSales.Application.Products.GetProduct;

public sealed record GetProductQuery(Guid ProductId) : IQuery<ProductResponse>;
