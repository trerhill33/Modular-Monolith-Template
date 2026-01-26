using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Orders.Application.Customers.CreateCustomer;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.CreateCustomer;

internal sealed class CreateCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCustomerAsync)
            .WithSummary("Create a new customer")
            .WithDescription("Creates a new customer with the specified name and email.")
            .Produces<CreateCustomerResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateCustomerAsync(
        CreateCustomerRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(request.Name, request.Email);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            id => Results.Created($"/customers/{id}", new CreateCustomerResponse(id)),
            ApiResults.Problem);
    }
}

public sealed record CreateCustomerRequest(string Name, string Email);

public sealed record CreateCustomerResponse(Guid Id);
