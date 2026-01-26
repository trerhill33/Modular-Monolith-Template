using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sales.Domain;
using ModularTemplate.Modules.Sales.Domain.Catalogs;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.UpdateCatalog;

internal sealed class UpdateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISalesModule> unitOfWork)
    : ICommandHandler<UpdateCatalogCommand>
{
    public async Task<Result> Handle(
        UpdateCatalogCommand request,
        CancellationToken cancellationToken)
    {
        Catalog? catalog = await catalogRepository.GetByIdAsync(request.CatalogId, cancellationToken);

        if (catalog is null)
        {
            return Result.Failure(CatalogErrors.NotFound(request.CatalogId));
        }

        Catalog.Update(catalog, request.Name, request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
