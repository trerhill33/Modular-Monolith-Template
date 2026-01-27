using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Sample.Application.Products.GetProduct;

namespace ModularTemplate.Modules.Sample.Presentation.Endpoints.Products.V1;

internal sealed class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/{productId:guid}", GetProductByIdAsync)
            .WithSummary("Get a product by ID")
            .WithDescription("Retrieves a product by its unique identifier.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetProductByIdAsync(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetProductQuery(productId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, ApiResults.Problem);
    }
}
