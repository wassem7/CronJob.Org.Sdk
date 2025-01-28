using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class Schedule
{
    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("expiresAt")]
    public long ExpiresAt { get; set; }

    [JsonPropertyName("exceptionDates")]
    public List<int> ExceptionDates { get; set; } = new();

    [JsonPropertyName("mdays")]
    public List<int> Mdays { get; set; } = new();

    [JsonPropertyName("hours")]
    public List<int> Hours { get; set; } = new();

    [JsonPropertyName("minutes")]
    public List<int> Minutes { get; set; } = new();

    [JsonPropertyName("months")]
    public List<int> Months { get; set; } = new();

    [JsonPropertyName("wdays")]
    public List<int> Wdays { get; set; } = new();
}
