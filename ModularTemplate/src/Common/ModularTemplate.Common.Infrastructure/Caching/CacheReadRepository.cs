using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Domain.Caching;
using ModularTemplate.Common.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace ModularTemplate.Common.Infrastructure.Caching;

/// <summary>
/// Base repository for cache projection entities.
/// Provides standard read operations with Guid as the ID type.
/// </summary>
/// <typeparam name="TEntity">The cache projection entity type.</typeparam>
/// <typeparam name="TDbContext">The DbContext type.</typeparam>
public abstract class CacheReadRepository<TEntity, TDbContext>(TDbContext dbContext)
    : ReadRepository<TEntity, Guid, TDbContext>(dbContext)
    where TEntity : class, ICacheProjection
    where TDbContext : DbContext
{
    /// <summary>
    /// Cache projections use Id as the identifier property.
    /// </summary>
    protected override Expression<Func<TEntity, Guid>> IdSelector => GetIdSelector();

    /// <summary>
    /// Gets the ID selector expression. Override if your entity uses a different ID property name.
    /// </summary>
    protected virtual Expression<Func<TEntity, Guid>> GetIdSelector()
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var idProperty = Expression.Property(parameter, "Id");
        return Expression.Lambda<Func<TEntity, Guid>>(idProperty, parameter);
    }
}
