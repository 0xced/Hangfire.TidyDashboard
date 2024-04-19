using System;
using System.Data.Common;
using System.Threading;
using Hangfire.SqlServer;

// ReSharper disable once CheckNamespace
namespace Hangfire;

/// <summary>
/// Extensions methods for <see cref="IGlobalConfiguration"/> to configure <see cref="TidySqlServerStorage"/> instead of <see cref="SqlServerStorage"/>.
/// </summary>
/// <seealso cref="Hangfire.SqlServerStorageExtensions"/>
public static class TidySqlServerStorageExtensions
{
    private static int _metricsInitialized;

    /// <summary>
    /// Configures SqlServerStorage from the provided connection string or the connection string with provided name pulled from the application config file.
    /// </summary>
    /// <param name="configuration">The <see cref="IGlobalConfiguration"/> to configure.</param>
    /// <param name="nameOrConnectionString">Either a SQL Server connection string or the name of a SQL Server connection string located in the connectionStrings node in the application config.</param>
    /// <exception cref="ArgumentNullException"><paramref name="nameOrConnectionString" /> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="nameOrConnectionString" /> argument is neither a valid SQL Server connection string nor the name of a connection string in the application config file.</exception>
    /// <returns>An <see cref="IGlobalConfiguration{SqlServerStorage}"/> for chaining configuration.</returns>
    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, string nameOrConnectionString)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(nameOrConnectionString);

        var storage = new TidySqlServerStorage(nameOrConnectionString);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    /// <summary>
    /// Configures SqlServerStorage from the provided SqlServerStorageOptions and either the provided connection string or the connection string with provided name pulled from the application config file.
    /// </summary>
    /// <param name="configuration">The <see cref="IGlobalConfiguration"/> to configure.</param>
    /// <param name="nameOrConnectionString">Either a SQL Server connection string or the name of a SQL Server connection string located in the connectionStrings node in the application config.</param>
    /// <param name="options">The SQL Server storage options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="nameOrConnectionString" /> argument is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="options" /> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="nameOrConnectionString" /> argument is neither a valid SQL Server connection string nor the name of a connection string in the application config file.</exception>
    /// <returns>An <see cref="IGlobalConfiguration{SqlServerStorage}"/> for chaining configuration.</returns>
    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, string nameOrConnectionString, SqlServerStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(nameOrConnectionString);
        ArgumentNullException.ThrowIfNull(options);

        var storage = new TidySqlServerStorage(nameOrConnectionString, options);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    /// <summary>
    /// Configures SqlServerStorage with a connection factory <see cref="T:System.Func`1" /> class that will be invoked to create new database connections for querying the data.
    /// </summary>
    /// <param name="configuration">The <see cref="IGlobalConfiguration"/> to configure.</param>
    /// <param name="connectionFactory">The <see cref="T:System.Func`1" /> class that will be invoked to create new database connections.</param>
    /// <returns>An <see cref="IGlobalConfiguration{SqlServerStorage}"/> for chaining configuration.</returns>
    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, Func<DbConnection> connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(connectionFactory);

        var storage = new TidySqlServerStorage(connectionFactory);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    /// <summary>
    /// Configures SqlServerStorage with a connection factory <see cref="T:System.Func`1" /> class that will be invoked to create new database connections for querying the data.
    /// </summary>
    /// <param name="configuration">The <see cref="IGlobalConfiguration"/> to configure.</param>
    /// <param name="connectionFactory">The <see cref="T:System.Func`1" /> class that will be invoked to create new database connections.</param>
    /// <param name="options">The SQL Server storage options.</param>
    /// <returns>An <see cref="IGlobalConfiguration{SqlServerStorage}"/> for chaining configuration.</returns>
    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, Func<DbConnection> connectionFactory, SqlServerStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(connectionFactory);
        ArgumentNullException.ThrowIfNull(options);

        var storage = new TidySqlServerStorage(connectionFactory, options);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    private static IGlobalConfiguration<TidySqlServerStorage> UseSqlServerStorageCommonMetrics(this IGlobalConfiguration<TidySqlServerStorage> configuration)
    {
        if (Interlocked.Exchange(ref _metricsInitialized, 1) == 0)
        {
            configuration.UseDashboardMetric(SqlServerStorage.SchemaVersion);
            configuration.UseDashboardMetric(SqlServerStorage.ActiveConnections);
            configuration.UseDashboardMetric(SqlServerStorage.TotalConnections);
            configuration.UseDashboardMetric(SqlServerStorage.ActiveTransactions);
            configuration.UseDashboardMetric(SqlServerStorage.DataFilesSize);
            configuration.UseDashboardMetric(SqlServerStorage.LogFilesSize);
        }

        return configuration;
    }
}