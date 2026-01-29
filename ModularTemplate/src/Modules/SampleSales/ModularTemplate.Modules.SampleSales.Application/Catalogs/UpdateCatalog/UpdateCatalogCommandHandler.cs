using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.SampleSales.Domain;
using ModularTemplate.Modules.SampleSales.Domain.Catalogs;

namespace ModularTemplate.Modules.SampleSales.Application.Catalogs.UpdateCatalog;

internal sealed class UpdateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISampleSalesModule> unitOfWork)
    : ICommandHandler<UpdateCatalogCommand>
{
    public async Task<Result> Handle(
        UpdateCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalog = await catalogRepository.GetByIdAsync(request.CatalogId, cancellationToken);

        if (catalog is null)
        {
            return Result.Failure(CatalogErrors.NotFound(request.CatalogId));
        }

        var updateResult = catalog.Update(request.Name, request.Description);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
