using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Sales.Application.Products.GetProduct;
using ModularTemplate.Modules.Sales.Application.Products.GetProducts;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Products.GetAllProducts;

internal sealed class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllProductsAsync)
            .WithSummary("Get all products")
            .WithDescription("Retrieves all products with optional limit.")
            .Produces<IReadOnlyCollection<ProductResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllProductsAsync(
        ISender sender,
        CancellationToken cancellationToken,
        int? limit = 100)
    {
        var query = new GetProductsQuery(limit);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, ApiResults.Problem);
    }
}
