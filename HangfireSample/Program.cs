using System.Runtime.InteropServices;
using Hangfire;
using HangfireSample;
using Testcontainers.MsSql;

await using var sqlContainer = new MsSqlBuilder().WithName("Hangfire.TidyDashboard").WithReuse(true).Build();

await sqlContainer.StartAsync();

await RunAsync(sqlContainer.GetConnectionString(), args);

return;

static async Task RunAsync(string connectionString, string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddHangfire(c =>
    {
        c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseTidySqlServerStorage(connectionString)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseFilter(new SampleFilter());
    });
    builder.Services.AddHangfireServer();
    var app = builder.Build();

    app.MapHangfireDashboard();

    var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<SampleJob>("sample-1", j => j.String(RuntimeInformation.RuntimeIdentifier), "0 12 * * *");
    recurringJobManager.AddOrUpdate<SampleJob>("sample-2", j => j.Object(default!), "0 12 * * *");

    await app.RunAsync();
}