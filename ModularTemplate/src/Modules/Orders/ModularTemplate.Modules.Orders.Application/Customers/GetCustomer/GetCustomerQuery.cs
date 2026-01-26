using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Orders.Application.Customers.GetCustomer;

public sealed record GetCustomerQuery(Guid CustomerId) : IQuery<CustomerResponse>;
