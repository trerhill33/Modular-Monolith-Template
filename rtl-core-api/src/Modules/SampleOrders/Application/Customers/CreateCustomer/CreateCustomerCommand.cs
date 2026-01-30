using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleOrders.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Email) : ICommand<Guid>;
