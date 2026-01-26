using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Orders.Domain;
using ModularTemplate.Modules.Orders.Domain.Customers;

namespace ModularTemplate.Modules.Orders.Application.Customers.UpdateCustomer;

internal sealed class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork<IOrdersModule> unitOfWork)
    : ICommandHandler<UpdateCustomerCommand>
{
    public async Task<Result> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        Customer? customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(CustomerErrors.NotFound(request.CustomerId));
        }

        Customer.Update(customer, request.Name, request.Email);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
