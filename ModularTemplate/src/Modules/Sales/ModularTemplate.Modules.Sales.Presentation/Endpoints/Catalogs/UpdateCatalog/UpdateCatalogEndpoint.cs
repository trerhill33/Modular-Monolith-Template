using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Sales.Application.Catalogs.UpdateCatalog;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.UpdateCatalog;

internal sealed class UpdateCatalogEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("/{catalogId:guid}", UpdateCatalogAsync)
            .WithSummary("Update a catalog")
            .WithDescription("Updates an existing catalog with the specified details.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> UpdateCatalogAsync(
        Guid catalogId,
        UpdateCatalogRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCatalogCommand(
            catalogId,
            request.Name,
            request.Description);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            () => Results.NoContent(),
            ApiResults.Problem);
    }
}

public sealed record UpdateCatalogRequest(string Name, string? Description);
