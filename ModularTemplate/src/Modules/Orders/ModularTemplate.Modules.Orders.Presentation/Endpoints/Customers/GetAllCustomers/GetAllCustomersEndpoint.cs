using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Common.Presentation.Results;
using ModularTemplate.Modules.Orders.Application.Customers.GetCustomer;
using ModularTemplate.Modules.Orders.Application.Customers.GetCustomers;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.GetAllCustomers;

internal sealed class GetAllCustomersEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllCustomersAsync)
            .WithSummary("Get all customers")
            .WithDescription("Retrieves all customers with optional limit.")
            .Produces<IReadOnlyCollection<CustomerResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllCustomersAsync(
        ISender sender,
        CancellationToken cancellationToken,
        int? limit = 100)
    {
        var query = new GetCustomersQuery(limit);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, ApiResults.Problem);
    }
}
