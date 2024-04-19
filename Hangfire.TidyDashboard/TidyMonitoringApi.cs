using System;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.States;
using Hangfire.Storage.Monitoring;

// ReSharper disable once CheckNamespace
namespace Hangfire.Storage;

/// <summary>
/// A wrapper around a <see cref="JobStorageMonitor"/> that tidies the <see cref="JobDetailsDto"/> objects returned by the <see cref="JobDetails"/> method.
/// </summary>
/// <param name="monitor">The <see cref="JobStorageMonitor"/> to wrap.</param>
public class TidyMonitoringApi(JobStorageMonitor monitor) : JobStorageMonitor
{
    static TidyMonitoringApi()
    {
        JobHistoryRenderer.Register(SucceededState.StateName, TidyJobHistoryRenderer.SucceededRenderer);
    }

    /// <inheritdoc />
    public override JobDetailsDto JobDetails(string jobId)
    {
        var jobDetails = monitor.JobDetails(jobId);

        foreach (var (key, value) in jobDetails.Properties)
        {
            if (value.StartsWith('\"'))
            {
                try
                {
                    jobDetails.Properties[key] = SerializationHelper.Deserialize<string>(value, SerializationOption.User);
                }
                catch
                {
                    // Ignore
                }
            }
        }

        return jobDetails;
    }

    /// <inheritdoc />
    public override IList<QueueWithTopEnqueuedJobsDto> Queues() => monitor.Queues();
    /// <inheritdoc />
    public override IList<ServerDto> Servers() => monitor.Servers();
    /// <inheritdoc />
    public override StatisticsDto GetStatistics() => monitor.GetStatistics();
    /// <inheritdoc />
    public override JobList<EnqueuedJobDto> EnqueuedJobs(string queue, int from, int perPage) => monitor.EnqueuedJobs(queue, from, perPage);
    /// <inheritdoc />
    public override JobList<FetchedJobDto> FetchedJobs(string queue, int from, int perPage) => monitor.FetchedJobs(queue, from, perPage);
    /// <inheritdoc />
    public override JobList<ProcessingJobDto> ProcessingJobs(int from, int count) => monitor.ProcessingJobs(from, count);
    /// <inheritdoc />
    public override JobList<ScheduledJobDto> ScheduledJobs(int from, int count) => monitor.ScheduledJobs(from, count);
    /// <inheritdoc />
    public override JobList<SucceededJobDto> SucceededJobs(int from, int count) => monitor.SucceededJobs(from, count);
    /// <inheritdoc />
    public override JobList<FailedJobDto> FailedJobs(int from, int count) => monitor.FailedJobs(from, count);
    /// <inheritdoc />
    public override JobList<DeletedJobDto> DeletedJobs(int from, int count) => monitor.DeletedJobs(from, count);
    /// <inheritdoc />
    public override JobList<AwaitingJobDto> AwaitingJobs(int from, int count) => monitor.AwaitingJobs(from, count);
    /// <inheritdoc />
    public override long ScheduledCount() => monitor.ScheduledCount();
    /// <inheritdoc />
    public override long EnqueuedCount(string queue) => monitor.EnqueuedCount(queue);
    /// <inheritdoc />
    public override long FetchedCount(string queue) => monitor.FetchedCount(queue);
    /// <inheritdoc />
    public override long FailedCount() => monitor.FailedCount();
    /// <inheritdoc />
    public override long ProcessingCount() => monitor.ProcessingCount();
    /// <inheritdoc />
    public override long SucceededListCount() => monitor.SucceededListCount();
    /// <inheritdoc />
    public override long DeletedListCount() => monitor.DeletedListCount();
    /// <inheritdoc />
    public override long AwaitingCount() => monitor.AwaitingCount();
    /// <inheritdoc />
    public override IDictionary<DateTime, long> SucceededByDatesCount() => monitor.SucceededByDatesCount();
    /// <inheritdoc />
    public override IDictionary<DateTime, long> FailedByDatesCount() => monitor.FailedByDatesCount();
    /// <inheritdoc />
    public override IDictionary<DateTime, long> DeletedByDatesCount() => monitor.DeletedByDatesCount();
    /// <inheritdoc />
    public override IDictionary<DateTime, long> HourlySucceededJobs() => monitor.HourlySucceededJobs();
    /// <inheritdoc />
    public override IDictionary<DateTime, long> HourlyFailedJobs() => monitor.HourlyFailedJobs();
    /// <inheritdoc />
    public override IDictionary<DateTime, long> HourlyDeletedJobs() => monitor.HourlyDeletedJobs();
}