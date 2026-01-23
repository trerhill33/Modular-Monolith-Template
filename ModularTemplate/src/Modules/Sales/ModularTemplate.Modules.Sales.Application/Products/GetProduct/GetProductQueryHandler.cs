using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sales.Domain.Products;

namespace ModularTemplate.Modules.Sales.Application.Products.GetProduct;

internal sealed class GetProductQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(ProductErrors.NotFound(request.ProductId));
        }

        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive,
            product.CreatedAtUtc,
            product.CreatedByUserId,
            product.ModifiedAtUtc,
            product.ModifiedByUserId);
    }
}
