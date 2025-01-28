using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class CronJobApiResponse
{
    [JsonPropertyName("jobId")]
    public long JobId { get; set; }
}