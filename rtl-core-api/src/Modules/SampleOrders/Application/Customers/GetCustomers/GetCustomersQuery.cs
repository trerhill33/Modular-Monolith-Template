using Rtl.Core.Application.Messaging;
using Rtl.Module.SampleOrders.Application.Customers.GetCustomer;

namespace Rtl.Module.SampleOrders.Application.Customers.GetCustomers;

public sealed record GetCustomersQuery(int? Limit = 100) : IQuery<IReadOnlyCollection<CustomerResponse>>;
