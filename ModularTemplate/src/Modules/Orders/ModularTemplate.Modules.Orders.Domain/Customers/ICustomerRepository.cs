using ModularTemplate.Common.Domain;

namespace ModularTemplate.Modules.Orders.Domain.Customers;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
