using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleOrders.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Email) : ICommand<Guid>;
