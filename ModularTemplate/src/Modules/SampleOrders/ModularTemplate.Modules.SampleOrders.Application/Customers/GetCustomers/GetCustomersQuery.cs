using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.SampleOrders.Application.Customers.GetCustomer;

namespace ModularTemplate.Modules.SampleOrders.Application.Customers.GetCustomers;

public sealed record GetCustomersQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CustomerResponse>>;
