namespace ModularTemplate.Common.Application.Data;

/// <summary>
/// Module-specific factory for creating database connections.
/// </summary>
/// <remarks>
/// <para>
/// This generic interface enables each module to have its own database connection factory,
/// allowing modules to connect to separate databases when deployed independently.
/// </para>
/// <para>
public interface IDbConnectionFactory<TModule> : IDbConnectionFactory
    where TModule : class
{
}
