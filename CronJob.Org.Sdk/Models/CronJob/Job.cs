using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class Job
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("saveResponses")]
    public bool SaveResponses { get; set; } = true;

    [JsonPropertyName("schedule")]
    public Schedule Schedule { get; set; }
}
