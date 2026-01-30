using Rtl.Core.Application.Messaging;
using Rtl.Core.Domain.Results;
using Rtl.Module.SampleOrders.Domain.Customers;

namespace Rtl.Module.SampleOrders.Application.Customers.GetCustomer;

internal sealed class GetCustomerQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomerQuery, CustomerResponse>
{
    public async Task<Result<CustomerResponse>> Handle(
        GetCustomerQuery request,
        CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerResponse>(CustomerErrors.NotFound(request.CustomerId));
        }

        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.CreatedAtUtc,
            customer.CreatedByUserId,
            customer.ModifiedAtUtc,
            customer.ModifiedByUserId);
    }
}
