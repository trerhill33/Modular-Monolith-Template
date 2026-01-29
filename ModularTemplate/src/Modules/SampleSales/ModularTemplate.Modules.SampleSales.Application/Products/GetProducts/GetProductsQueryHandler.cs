using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleSales.Application.Products.GetProduct;
using ModularTemplate.Modules.SampleSales.Domain.Products;

namespace ModularTemplate.Modules.SampleSales.Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductsQuery, IReadOnlyCollection<ProductResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProductResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Product> products = await productRepository.GetAllAsync(
            request.Limit,
            cancellationToken);

        var response = products.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price.Amount,
            p.Price.Currency,
            p.IsActive,
            p.CreatedAtUtc,
            p.CreatedByUserId,
            p.ModifiedAtUtc,
            p.ModifiedByUserId)).ToList();

        return response;
    }
}
