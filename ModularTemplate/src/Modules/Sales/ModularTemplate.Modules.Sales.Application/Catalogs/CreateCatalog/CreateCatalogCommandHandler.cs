using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sales.Domain;
using ModularTemplate.Modules.Sales.Domain.Catalogs;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.CreateCatalog;

internal sealed class CreateCatalogCommandHandler(
    ICatalogRepository catalogRepository,
    IUnitOfWork<ISalesModule> unitOfWork,
    ILogger<CreateCatalogCommandHandler> logger)
    : ICommandHandler<CreateCatalogCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCatalogCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("[CreateCatalog] Starting handler for Name={Name}, Description={Description}",
            request.Name, request.Description);

        var catalog = Catalog.Create(request.Name, request.Description);

        logger.LogInformation("[CreateCatalog] Catalog entity created with Id={CatalogId}, CreatedAtUtc={CreatedAtUtc}, ModifiedAtUtc={ModifiedAtUtc}",
            catalog.Id, catalog.CreatedAtUtc, catalog.ModifiedAtUtc);

        catalogRepository.Add(catalog);

        logger.LogInformation("[CreateCatalog] Catalog added to repository, about to call SaveChangesAsync");

        try
        {
            var rowsAffected = await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("[CreateCatalog] SaveChangesAsync completed. RowsAffected={RowsAffected}, CatalogId={CatalogId}, CreatedAtUtc={CreatedAtUtc}, ModifiedAtUtc={ModifiedAtUtc}",
                rowsAffected, catalog.Id, catalog.CreatedAtUtc, catalog.ModifiedAtUtc);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CreateCatalog] SaveChangesAsync FAILED with exception: {Message}", ex.Message);
            throw;
        }

        return catalog.Id;
    }
}
