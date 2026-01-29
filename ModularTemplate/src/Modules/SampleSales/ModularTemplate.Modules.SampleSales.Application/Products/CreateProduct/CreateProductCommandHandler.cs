using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleSales.Domain;
using ModularTemplate.Modules.SampleSales.Domain.Products;

namespace ModularTemplate.Modules.SampleSales.Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork<ISampleSalesModule> unitOfWork)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var productResult = Product.Create(request.Name, request.Description, request.Price);

        if (productResult.IsFailure)
        {
            return Result.Failure<Guid>(productResult.Error);
        }

        productRepository.Add(productResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return productResult.Value.Id;
    }
}
