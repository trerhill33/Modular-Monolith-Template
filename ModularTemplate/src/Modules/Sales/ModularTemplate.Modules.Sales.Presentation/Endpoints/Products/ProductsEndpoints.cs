using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Products.CreateProduct;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Products.GetAllProducts;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Products.GetProductById;
using ModularTemplate.Modules.Sales.Presentation.Endpoints.Products.UpdateProduct;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Products;

internal sealed class ProductsEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        new GetAllProductsEndpoint(),
        new GetProductByIdEndpoint(),
        new CreateProductEndpoint(),
        new UpdateProductEndpoint()
    ];
}
