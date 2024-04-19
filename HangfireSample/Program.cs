using System.Runtime.InteropServices;
using Hangfire;
using Hangfire.Client;
using Hangfire.Server;
using Testcontainers.MsSql;

await using var sqlContainer = new MsSqlBuilder()
    .WithReuse(true)
    .WithLabel("reuse-id", "Hangfire.TidyDashboard")
    .WithName("Hangfire.TidyDashboard")
    .Build();

await sqlContainer.StartAsync();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(c =>
{
    c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseTidySqlServerStorage(sqlContainer.GetConnectionString())
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

internal class SampleJob
{
    public string String(string rid)
    {
        return """
               The file "test.txt" was saved to
               C:\data\export\test.txt
               """;
    }

    public object Object(PerformContext context)
    {
        var job = context.Storage.GetMonitoringApi().JobDetails(context.BackgroundJob.Id);
        if (job.Properties.TryGetValue("RetryCount", out var retryCount) && int.Parse(retryCount) > 1)
            return $"✅ After {retryCount} retries";

        if (int.TryParse(context.BackgroundJob.Id, out var id) && id % 2 != 0)
            throw new NotSupportedException("Odd jobs must be retried");

        return new { Success = true, Value = 42 };
    }
}

internal class SampleFilter : IClientFilter
{
    public void OnCreating(CreatingContext context)
    {
        context.Parameters["Anonymous"] = new { π = 3.14m };
    }

    public void OnCreated(CreatedContext context)
    {
    }
}