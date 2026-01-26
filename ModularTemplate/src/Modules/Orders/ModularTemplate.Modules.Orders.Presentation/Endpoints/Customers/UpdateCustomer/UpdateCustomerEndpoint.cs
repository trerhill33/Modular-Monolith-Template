using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Orders.Application.Customers.UpdateCustomer;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.UpdateCustomer;

internal sealed class UpdateCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("/{customerId:guid}", UpdateCustomerAsync)
            .WithSummary("Update a customer")
            .WithDescription("Updates an existing customer with the specified details.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> UpdateCustomerAsync(
        Guid customerId,
        UpdateCustomerRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerCommand(
            customerId,
            request.Name,
            request.Email);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            () => Results.NoContent(),
            ApiResults.Problem);
    }
}

public sealed record UpdateCustomerRequest(string Name, string Email);
