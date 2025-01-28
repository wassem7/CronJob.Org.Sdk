using System.Net;

namespace CronJob.Org.Sdk.Models.CronJob;

public class ScheduleJobResponse
{
    public bool Success { get; set; }
    
    public string Message { get; set; }
    
    public HttpStatusCode? StatusCode { get; set; }
    
    public long? JobId { get; set; }
}