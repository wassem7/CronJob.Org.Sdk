using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class CronJobApiRequest
{
    [JsonPropertyName("job")] 
    public Job Job { get; set; }
}
