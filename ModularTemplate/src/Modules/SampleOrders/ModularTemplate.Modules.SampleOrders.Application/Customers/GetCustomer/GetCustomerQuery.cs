using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleOrders.Application.Customers.GetCustomer;

public sealed record GetCustomerQuery(Guid CustomerId) : IQuery<CustomerResponse>;
