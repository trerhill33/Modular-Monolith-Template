using System.Data.Common;

namespace ModularTemplate.Common.Application.Data;

/// <summary>
/// Factory for creating database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Opens and returns a database connection.
    /// </summary>
    ValueTask<DbConnection> OpenConnectionAsync();
}
