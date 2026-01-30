using Rtl.Core.Application.Messaging;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Domain.Results;
using Rtl.Module.SampleSales.Domain;
using Rtl.Module.SampleSales.Domain.Catalogs;

namespace Rtl.Module.SampleSales.Application.Catalogs.CreateCatalog;

internal sealed class CreateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISampleSalesModule> unitOfWork)
    : ICommandHandler<CreateCatalogCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogResult = Catalog.Create(request.Name, request.Description);

        if (catalogResult.IsFailure)
        {
            return Result.Failure<Guid>(catalogResult.Error);
        }

        catalogRepository.Add(catalogResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return catalogResult.Value.Id;
    }
}
