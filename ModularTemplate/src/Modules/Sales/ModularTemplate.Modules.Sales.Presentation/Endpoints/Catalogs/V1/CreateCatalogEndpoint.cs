using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Sales.Application.Catalogs.CreateCatalog;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.V1;

internal sealed class CreateCatalogEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCatalogAsync)
            .WithSummary("Create a new catalog")
            .WithDescription("Creates a new catalog with the specified name and description.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<CreateCatalogResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateCatalogAsync(
        CreateCatalogRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateCatalogCommand(request.Name, request.Description);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            id => Results.Created($"/catalogs/{id}", new CreateCatalogResponse(id)),
            ApiResults.Problem);
    }
}

public sealed record CreateCatalogRequest(string Name, string? Description);

public sealed record CreateCatalogResponse(Guid Id);
