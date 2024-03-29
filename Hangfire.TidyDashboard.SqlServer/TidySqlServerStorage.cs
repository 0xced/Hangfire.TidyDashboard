using System;
using System.Data.Common;
using Hangfire.Storage;

// ReSharper disable once CheckNamespace
namespace Hangfire.SqlServer;

public class TidySqlServerStorage : SqlServerStorage
{
    public TidySqlServerStorage(string nameOrConnectionString) : base(nameOrConnectionString) {}

    public TidySqlServerStorage(string nameOrConnectionString, SqlServerStorageOptions options) : base(nameOrConnectionString, options) {}

    public TidySqlServerStorage(Func<DbConnection> connectionFactory) : base(connectionFactory) {}

    public TidySqlServerStorage(Func<DbConnection> connectionFactory, SqlServerStorageOptions options) : base(connectionFactory, options) {}

    public override IMonitoringApi GetMonitoringApi() => new TidyMonitoringApi(base.GetMonitoringApi());
}