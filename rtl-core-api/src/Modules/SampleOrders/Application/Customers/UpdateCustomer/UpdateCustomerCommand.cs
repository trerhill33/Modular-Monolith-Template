using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleOrders.Application.Customers.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string Name,
    string Email) : ICommand;
