using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class JobDetailsResponse
{
    [JsonPropertyName("jobDetails")]
    public JobDetailsInfo JobDetails { get; set; }
}

public class JobDetailsInfo : JobDetails
{
    [JsonPropertyName("auth")]
    public AuthInfo Auth { get; set; }

    [JsonPropertyName("notification")]
    public NotificationInfo Notification { get; set; }

    [JsonPropertyName("extendedData")]
    public ExtendedDataInfo ExtendedData { get; set; }
}

public class AuthInfo
{
    [JsonPropertyName("enable")]
    public bool Enable { get; set; }

    [JsonPropertyName("user")]
    public string User { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }
}

public class NotificationInfo
{
    [JsonPropertyName("onFailure")]
    public bool OnFailure { get; set; }

    [JsonPropertyName("onSuccess")]
    public bool OnSuccess { get; set; }

    [JsonPropertyName("onDisable")]
    public bool OnDisable { get; set; }
}

public class ExtendedDataInfo
{
    [JsonPropertyName("headers")]
    public List<object> Headers { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }
}
