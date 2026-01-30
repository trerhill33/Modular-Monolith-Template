using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rtl.Core.Presentation.Endpoints;
using Rtl.Core.Presentation.Results;
using Rtl.Module.SampleOrders.Application.Customers.GetCustomer;
using Rtl.Module.SampleOrders.Application.Customers.GetCustomers;

namespace Rtl.Module.SampleOrders.Presentation.Endpoints.Customers.V1;

internal sealed class GetAllCustomersEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllCustomersAsync)
            .WithSummary("Get all customers")
            .WithDescription("Retrieves all customers with optional limit.")
            .MapToApiVersion(new ApiVersion(1, 0))
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
