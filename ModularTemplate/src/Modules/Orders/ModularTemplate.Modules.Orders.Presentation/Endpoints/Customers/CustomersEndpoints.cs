using ModularTemplate.Common.Presentation.Endpoints;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.CreateCustomer;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.GetAllCustomers;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.GetCustomerById;
using ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers.UpdateCustomer;

namespace ModularTemplate.Modules.Orders.Presentation.Endpoints.Customers;

internal sealed class CustomersEndpoints : ResourceEndpoints
{
    protected override IEndpoint[] Endpoints =>
    [
        new GetAllCustomersEndpoint(),
        new GetCustomerByIdEndpoint(),
        new CreateCustomerEndpoint(),
        new UpdateCustomerEndpoint()
    ];
}
