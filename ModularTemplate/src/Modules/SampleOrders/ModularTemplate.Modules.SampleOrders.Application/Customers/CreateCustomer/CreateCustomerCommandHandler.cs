using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleOrders.Domain;
using ModularTemplate.Modules.SampleOrders.Domain.Customers;

namespace ModularTemplate.Modules.SampleOrders.Application.Customers.CreateCustomer;

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
