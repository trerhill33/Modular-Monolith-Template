namespace ModularTemplate.Common.Application.Data;

/// <summary>
/// Represents a unit of work for coordinating persistence operations.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
