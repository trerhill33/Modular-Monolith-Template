using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Application.Persistence;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sample.Domain;
using ModularTemplate.Modules.Sample.Domain.Catalogs;

namespace ModularTemplate.Modules.Sample.Application.Catalogs.CreateCatalog;

internal sealed class CreateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISampleModule> unitOfWork)
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
