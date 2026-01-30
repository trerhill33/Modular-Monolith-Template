using Rtl.Core.Application.Messaging;
using Rtl.Module.SampleSales.Application.Products.GetProduct;

namespace Rtl.Module.SampleSales.Application.Products.GetProducts;

public sealed record GetProductsQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<ProductResponse>>;
