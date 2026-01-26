using ModularTemplate.Common.Domain.Entities;
using ModularTemplate.Modules.Orders.Domain.Customers.Events;

namespace ModularTemplate.Modules.Orders.Domain.Customers;

public sealed class Customer : SoftDeletableEntity
{
    private Customer()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public static Customer Create(string name, string email)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email
        };

        customer.Raise(new CustomerCreatedDomainEvent(customer.Id));

        return customer;
    }

    public static void Update(Customer customer, string name, string email)
    {
        customer.Name = name;
        customer.Email = email;

        customer.Raise(new CustomerUpdatedDomainEvent(customer.Id));
    }
}
