using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sales.Domain;
using ModularTemplate.Modules.Sales.Domain.Catalogs;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.CreateCatalog;

internal sealed class CreateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISalesModule> unitOfWork)
    : ICommandHandler<CreateCatalogCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalog = Catalog.Create(request.Name, request.Description);

        catalogRepository.Add(catalog);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return catalog.Id;
    }
}
