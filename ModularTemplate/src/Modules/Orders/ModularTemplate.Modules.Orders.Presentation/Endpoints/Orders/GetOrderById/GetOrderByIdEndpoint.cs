using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Orders.Application.Orders.GetOrder;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders.GetOrderById;

internal sealed class GetOrderByIdEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/{orderId:guid}", GetOrderByIdAsync)
            .WithSummary("Get an order by ID")
            .WithDescription("Retrieves an order by its unique identifier.")
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetOrderByIdAsync(
        Guid orderId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderQuery(orderId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, ApiResults.Problem);
    }
}
