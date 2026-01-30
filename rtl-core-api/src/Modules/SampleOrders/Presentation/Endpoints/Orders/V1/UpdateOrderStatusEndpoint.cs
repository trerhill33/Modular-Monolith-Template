using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rtl.Core.Presentation.Endpoints;
using Rtl.Core.Presentation.Results;
using Rtl.Module.SampleOrders.Application.Orders.UpdateOrderStatus;
using Rtl.Module.SampleOrders.Domain.Orders;

namespace Rtl.Module.SampleOrders.Presentation.Endpoints.Orders.V1;

internal sealed class UpdateOrderStatusEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPatch("/{orderId:guid}/status", UpdateOrderStatusAsync)
            .WithSummary("Update order status")
            .WithDescription("Updates the status of an existing order.")
            .MapToApiVersion(new ApiVersion(1, 0))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> UpdateOrderStatusAsync(
        Guid orderId,
        UpdateOrderStatusRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderStatusCommand(orderId, request.Status);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            () => Results.NoContent(),
            ApiResults.Problem);
    }
}

public sealed record UpdateOrderStatusRequest(OrderStatus Status);
