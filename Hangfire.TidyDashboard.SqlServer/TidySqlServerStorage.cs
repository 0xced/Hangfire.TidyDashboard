using System;
using System.Data.Common;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;

// ReSharper disable once CheckNamespace
namespace Hangfire.SqlServer;

/// <summary>
/// A <see cref="SqlServerStorage"/> subclass whose <see cref="GetMonitoringApi"/> method returns a tidy objects.
/// </summary>
public class TidySqlServerStorage : SqlServerStorage
{
    /// <inheritdoc />
    public TidySqlServerStorage(string nameOrConnectionString) : base(nameOrConnectionString) {}

    /// <inheritdoc />
    public TidySqlServerStorage(string nameOrConnectionString, SqlServerStorageOptions options) : base(nameOrConnectionString, options) {}

    /// <inheritdoc />
    public TidySqlServerStorage(Func<DbConnection> connectionFactory) : base(connectionFactory) {}

    /// <inheritdoc />
    public TidySqlServerStorage(Func<DbConnection> connectionFactory, SqlServerStorageOptions options) : base(connectionFactory, options) {}

    /// <summary>
    /// Gets an <see cref="IMonitoringApi"/> that returns tidy <see cref="JobDetailsDto"/> objects.
    /// </summary>
    public override IMonitoringApi GetMonitoringApi() => new TidyMonitoringApi((JobStorageMonitor)base.GetMonitoringApi());
}