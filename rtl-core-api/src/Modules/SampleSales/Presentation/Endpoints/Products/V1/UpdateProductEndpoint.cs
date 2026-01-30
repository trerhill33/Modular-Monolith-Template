using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rtl.Core.Presentation.Endpoints;
using Rtl.Core.Presentation.Results;
using Rtl.Module.SampleSales.Application.Products.UpdateProduct;

namespace Rtl.Module.SampleSales.Presentation.Endpoints.Products.V1;

internal sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}", UpdateProductAsync)
            .WithSummary("Update a product")
            .WithDescription("Updates an existing product with the specified details.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> UpdateProductAsync(
        Guid productId,
        UpdateProductRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            productId,
            request.Name,
            request.Description,
            request.Price,
            request.IsActive);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            () => Results.NoContent(),
            ApiResults.Problem);
    }
}

public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, bool IsActive);
