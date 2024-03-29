using System;
using System.Data.Common;
using System.Threading;
using Hangfire.SqlServer;

// ReSharper disable once CheckNamespace
namespace Hangfire;

public static class TidySqlServerStorageExtensions
{
    private static int _metricsInitialized;

    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, string nameOrConnectionString)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(nameOrConnectionString);

        var storage = new TidySqlServerStorage(nameOrConnectionString);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, string nameOrConnectionString, SqlServerStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(nameOrConnectionString);
        ArgumentNullException.ThrowIfNull(options);

        var storage = new TidySqlServerStorage(nameOrConnectionString, options);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, Func<DbConnection> connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(connectionFactory);

        var storage = new TidySqlServerStorage(connectionFactory);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    public static IGlobalConfiguration<SqlServerStorage> UseTidySqlServerStorage(this IGlobalConfiguration configuration, Func<DbConnection> connectionFactory, SqlServerStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(connectionFactory);
        ArgumentNullException.ThrowIfNull(options);

        var storage = new TidySqlServerStorage(connectionFactory, options);
        return configuration.UseStorage(storage).UseSqlServerStorageCommonMetrics();
    }

    private static IGlobalConfiguration<SqlServerStorage> UseSqlServerStorageCommonMetrics(this IGlobalConfiguration<SqlServerStorage> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

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