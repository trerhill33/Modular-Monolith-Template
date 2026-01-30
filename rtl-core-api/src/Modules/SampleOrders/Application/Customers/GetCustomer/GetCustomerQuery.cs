using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleOrders.Application.Customers.GetCustomer;

public sealed record GetCustomerQuery(Guid CustomerId) : IQuery<CustomerResponse>;
