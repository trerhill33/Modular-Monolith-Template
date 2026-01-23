using ModularTemplate.Common.Application.Data;
using Npgsql;
using System.Data.Common;

namespace ModularTemplate.Common.Infrastructure.Persistence;

/// <summary>
/// PostgreSQL implementation of IDbConnectionFactory.
/// </summary>
internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync() =>
        await dataSource.OpenConnectionAsync();
}
