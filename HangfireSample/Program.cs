using Hangfire;
using Testcontainers.MsSql;

await using var sqlContainer = new MsSqlBuilder().WithReuse(true).WithName("HangfireSample").Build();
await sqlContainer.StartAsync();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(c =>
{
    c.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseTidySqlServerStorage(sqlContainer.GetConnectionString())
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings();
});
builder.Services.AddHangfireServer();
var app = builder.Build();

app.MapHangfireDashboard();
app.MapPost("/", () => BackgroundJob.Enqueue<SampleJob>(job => job.Run()));

await app.RunAsync();

internal class SampleJob
{
    public string Run()
    {
        return """
               The file "test.txt" was saved to
               C:\data\export\test.txt
               """;
    }
}