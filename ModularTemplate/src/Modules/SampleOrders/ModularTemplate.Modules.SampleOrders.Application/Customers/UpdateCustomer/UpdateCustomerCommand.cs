using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleOrders.Application.Customers.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string Name,
    string Email) : ICommand;
