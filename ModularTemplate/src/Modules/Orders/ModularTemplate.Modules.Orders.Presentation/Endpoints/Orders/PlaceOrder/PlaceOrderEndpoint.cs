using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Orders.Application.Orders.PlaceOrder;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Orders.PlaceOrder;

internal sealed class PlaceOrderEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", PlaceOrderAsync)
            .WithSummary("Place a new order")
            .WithDescription("Places a new order for a product.")
            .Produces<PlaceOrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> PlaceOrderAsync(
        PlaceOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new PlaceOrderCommand(request.ProductId, request.Quantity);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            id => Results.Created($"/orders/{id}", new PlaceOrderResponse(id)),
            ApiResults.Problem);
    }
}

public sealed record PlaceOrderRequest(Guid ProductId, int Quantity);

public sealed record PlaceOrderResponse(Guid OrderId);
