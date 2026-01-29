using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrder;
using ModularTemplate.Modules.SampleOrders.Application.Orders.GetOrders;

namespace ModularTemplate.Modules.SampleOrders.Presentation.Endpoints.Orders.V1;

internal sealed class GetAllOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllOrdersAsync)
            .WithSummary("Get all orders")
            .WithDescription("Retrieves all orders with optional limit.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces<IReadOnlyCollection<OrderResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllOrdersAsync(
        ISender sender,
        CancellationToken cancellationToken,
        int? limit = 100)
    {
        var query = new GetOrdersQuery(limit);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, ApiResults.Problem);
    }
}
