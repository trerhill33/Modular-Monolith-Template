using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.SampleSales.Application.Products.CreateProduct;

namespace ModularTemplate.Modules.SampleSales.Presentation.Endpoints.Products.V1;

internal sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", CreateProductAsync)
            .WithSummary("Create a new product")
            .WithDescription("Creates a new product with the specified name, description, and price.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateProductAsync(
        CreateProductRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Description, request.Price);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            id => Results.Created($"/products/{id}", new CreateProductResponse(id)),
            ApiResults.Problem);
    }
}

public sealed record CreateProductRequest(string Name, string? Description, decimal Price);

public sealed record CreateProductResponse(Guid Id);
