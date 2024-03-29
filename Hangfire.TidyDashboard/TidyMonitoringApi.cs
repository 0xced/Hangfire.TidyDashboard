using System;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.States;
using Hangfire.Storage.Monitoring;

// ReSharper disable once CheckNamespace
namespace Hangfire.Storage;

public class TidyMonitoringApi(IMonitoringApi monitoringApi) : IMonitoringApi
{
    static TidyMonitoringApi()
    {
        JobHistoryRenderer.Register(SucceededState.StateName, TidyHangfireDisplay.SucceededRenderer);
    }

    JobDetailsDto IMonitoringApi.JobDetails(string jobId)
    {
        var jobDetails = monitoringApi.JobDetails(jobId);

        foreach (var (key, value) in jobDetails.Properties)
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

        return jobDetails;
    }

    IList<QueueWithTopEnqueuedJobsDto> IMonitoringApi.Queues() => monitoringApi.Queues();
    IList<ServerDto> IMonitoringApi.Servers() => monitoringApi.Servers();
    StatisticsDto IMonitoringApi.GetStatistics() => monitoringApi.GetStatistics();
    JobList<EnqueuedJobDto> IMonitoringApi.EnqueuedJobs(string queue, int from, int perPage) => monitoringApi.EnqueuedJobs(queue, from, perPage);
    JobList<FetchedJobDto> IMonitoringApi.FetchedJobs(string queue, int from, int perPage) => monitoringApi.FetchedJobs(queue, from, perPage);
    JobList<ProcessingJobDto> IMonitoringApi.ProcessingJobs(int from, int count) => monitoringApi.ProcessingJobs(from, count);
    JobList<ScheduledJobDto> IMonitoringApi.ScheduledJobs(int from, int count) => monitoringApi.ScheduledJobs(from, count);
    JobList<SucceededJobDto> IMonitoringApi.SucceededJobs(int from, int count) => monitoringApi.SucceededJobs(from, count);
    JobList<FailedJobDto> IMonitoringApi.FailedJobs(int from, int count) => monitoringApi.FailedJobs(from, count);
    JobList<DeletedJobDto> IMonitoringApi.DeletedJobs(int from, int count) => monitoringApi.DeletedJobs(from, count);
    long IMonitoringApi.ScheduledCount() => monitoringApi.ScheduledCount();
    long IMonitoringApi.EnqueuedCount(string queue) => monitoringApi.EnqueuedCount(queue);
    long IMonitoringApi.FetchedCount(string queue) => monitoringApi.FetchedCount(queue);
    long IMonitoringApi.FailedCount() => monitoringApi.FailedCount();
    long IMonitoringApi.ProcessingCount() => monitoringApi.ProcessingCount();
    long IMonitoringApi.SucceededListCount() => monitoringApi.SucceededListCount();
    long IMonitoringApi.DeletedListCount() => monitoringApi.DeletedListCount();
    IDictionary<DateTime, long> IMonitoringApi.SucceededByDatesCount() => monitoringApi.SucceededByDatesCount();
    IDictionary<DateTime, long> IMonitoringApi.FailedByDatesCount() => monitoringApi.FailedByDatesCount();
    IDictionary<DateTime, long> IMonitoringApi.HourlySucceededJobs() => monitoringApi.HourlySucceededJobs();
    IDictionary<DateTime, long> IMonitoringApi.HourlyFailedJobs() => monitoringApi.HourlyFailedJobs();
}