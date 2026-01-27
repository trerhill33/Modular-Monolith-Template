using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Domain.Products;

namespace ModularTemplate.Modules.Sample.Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork<ISampleModule> unitOfWork)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, request.Price);

        productRepository.Add(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
