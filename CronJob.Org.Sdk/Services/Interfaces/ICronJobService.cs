using CronJob.Org.Sdk.Models.CronJob;

namespace CronJob.Org.Sdk.Services.Interfaces;

public interface ICronJobService
{
    Task<ScheduleJobResponse> ScheduleJobAsync(CronJobScheduleRequest request);
    
    Task<GetJobsResponse> GetAllJobsAsync();
    
    Task<JobDetailsResponse> GetJobDetailsAsync(long jobId);
    
    Task<bool> DeleteJobAsync(long jobId);
}
