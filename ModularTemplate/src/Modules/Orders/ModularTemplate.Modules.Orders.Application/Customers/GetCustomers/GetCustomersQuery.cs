using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Modules.Orders.Application.Customers.GetCustomer;

namespace ModularTemplate.Modules.Orders.Application.Customers.GetCustomers;

public sealed record GetCustomersQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CustomerResponse>>;
