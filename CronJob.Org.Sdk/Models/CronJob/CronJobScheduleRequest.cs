using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class CronJobScheduleRequest
{
    [JsonPropertyName("webhookUrl")]
    public string WebhookUrl { get; set; }

    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; } = "UTC";

    [JsonPropertyName("scheduleType")]
    public JobScheduleType ScheduleType { get; set; }

    // For recurring jobs
    [JsonPropertyName("hour")]
    public int? Hour { get; set; }

    [JsonPropertyName("minute")]
    public int? Minute { get; set; }

    // For interval-based jobs
    [JsonPropertyName("minuteInterval")]
    public int? MinuteInterval { get; set; }

    // For one-time jobs
    [JsonPropertyName("executionTime")]
    public DateTime? ExecutionTime { get; set; }
}
