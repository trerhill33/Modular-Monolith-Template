using Rtl.Core.Domain;

namespace Rtl.Module.SampleOrders.Domain.Customers;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
