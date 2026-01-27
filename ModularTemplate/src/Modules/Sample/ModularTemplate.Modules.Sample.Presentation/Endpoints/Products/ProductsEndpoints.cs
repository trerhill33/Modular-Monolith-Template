using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sample.Presentation.Endpoints.Products.V1;

namespace ModularTemplate.Modules.Sample.Presentation.Endpoints.Products;

internal sealed class ProductsEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        // V1 endpoints
        new V1.GetAllProductsEndpoint(),
        new V1.GetProductByIdEndpoint(),
        new V1.CreateProductEndpoint(),
        new V1.UpdateProductEndpoint()
    ];
}
