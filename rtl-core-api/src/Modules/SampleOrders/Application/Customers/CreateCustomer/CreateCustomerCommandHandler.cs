using Rtl.Core.Application.Messaging;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain.Results;
using Rtl.Module.SampleOrders.Domain;
using Rtl.Module.SampleOrders.Domain.Customers;

namespace Rtl.Module.SampleOrders.Application.Customers.CreateCustomer;

internal sealed class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork<ISampleOrdersModule> unitOfWork)
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customerResult = Customer.Create(request.Name, request.Email);

        if (customerResult.IsFailure)
        {
            return Result.Failure<Guid>(customerResult.Error);
        }

        customerRepository.Add(customerResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return customerResult.Value.Id;
    }
}
