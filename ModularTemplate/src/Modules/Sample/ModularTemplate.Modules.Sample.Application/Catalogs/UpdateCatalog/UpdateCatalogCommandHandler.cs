using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Domain.Catalogs;

namespace ModularTemplate.Modules.Sample.Application.Catalogs.UpdateCatalog;

internal sealed class UpdateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISampleModule> unitOfWork)
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

        Catalog.Update(catalog, request.Name, request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
