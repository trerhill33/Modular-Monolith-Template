using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleOrders.Application.Customers.GetCustomer;
using ModularTemplate.Modules.SampleOrders.Domain.Customers;

namespace ModularTemplate.Modules.SampleOrders.Application.Customers.GetCustomers;

internal sealed class GetCustomersQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomersQuery, IReadOnlyCollection<CustomerResponse>>
{
    public async Task<Result<IReadOnlyCollection<CustomerResponse>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Customer> customers = await customerRepository.GetAllAsync(
            request.Limit,
            cancellationToken);

        var response = customers.Select(c => new CustomerResponse(
            c.Id,
            c.Name,
            c.Email,
            c.CreatedAtUtc,
            c.CreatedByUserId,
            c.ModifiedAtUtc,
            c.ModifiedByUserId)).ToList();

        return response;
    }
}
