using ModularTemplate.Common.Application.Persistence;
using Npgsql;
using System.Data.Common;

namespace ModularTemplate.Common.Infrastructure.Persistence;

/// <summary>
/// Module-specific PostgreSQL implementation of <see cref="IDbConnectionFactory{TModule}"/>.
/// </summary>
/// <remarks>
/// <para>
internal sealed class DbConnectionFactory<TModule>(NpgsqlDataSource dataSource)
    : IDbConnectionFactory<TModule>
    where TModule : class
{
    public async ValueTask<DbConnection> OpenConnectionAsync() =>
        await dataSource.OpenConnectionAsync();
}
