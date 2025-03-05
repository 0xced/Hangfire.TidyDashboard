using Hangfire.Server;

namespace HangfireSample;

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