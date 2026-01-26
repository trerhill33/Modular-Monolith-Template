using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.V1;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers;

internal sealed class CustomersEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        // V1 endpoints
        new V1.GetAllCustomersEndpoint(),
        new V1.GetCustomerByIdEndpoint(),
        new V1.CreateCustomerEndpoint(),
        new V1.UpdateCustomerEndpoint()
    ];
}
