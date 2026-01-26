using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Sales.Application.Catalogs.GetCatalog;
using ModularTemplate.Modules.Sales.Application.Catalogs.GetCatalogs;

namespace ModularTemplate.Modules.Sales.Presentation.Endpoints.Catalogs.V1;

/// <summary>
/// V1 endpoint: Returns a simple list of catalogs.
/// </summary>
internal sealed class GetAllCatalogsEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllCatalogsAsync)
            .WithSummary("Get all catalogs (v1)")
            .WithDescription("Retrieves all catalogs with optional limit. Returns a simple array.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<IReadOnlyCollection<CatalogResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllCatalogsAsync(
        ISender sender,
        CancellationToken cancellationToken,
        int? limit = 100)
    {
        var query = new GetCatalogsQuery(limit);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, ApiResults.Problem);
    }
}
