using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Orders.Domain;
using ModularTemplate.Modules.Orders.Domain.Customers;

namespace ModularTemplate.Modules.Orders.Application.Customers.CreateCustomer;

internal sealed class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork<IOrdersModule> unitOfWork)
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = Customer.Create(request.Name, request.Email);

        customerRepository.Add(customer);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
