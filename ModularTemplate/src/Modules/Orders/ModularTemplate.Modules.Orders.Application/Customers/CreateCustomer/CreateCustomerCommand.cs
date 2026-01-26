using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Orders.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Email) : ICommand<Guid>;
