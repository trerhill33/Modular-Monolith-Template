namespace ModularTemplate.Modules.Orders.Application.Customers.GetCustomer;

public sealed record CustomerResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAtUtc,
    Guid CreatedByUserId,
    DateTime? ModifiedAtUtc,
    Guid? ModifiedByUserId);
