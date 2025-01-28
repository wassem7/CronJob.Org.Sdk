using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class GetJobsResponse
{
    [JsonPropertyName("jobs")]
    public List<JobDetails> Jobs { get; set; }

    [JsonPropertyName("someFailed")]
    public bool SomeFailed { get; set; }
}



public class JobDetails
{
    [JsonPropertyName("jobId")]
    public long JobId { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("saveResponses")]
    public bool SaveResponses { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("lastStatus")]
    public int LastStatus { get; set; }

    [JsonPropertyName("lastDuration")]
    public int LastDuration { get; set; }

    [JsonPropertyName("lastExecution")]
    public long LastExecution { get; set; }

    [JsonPropertyName("nextExecution")]
    public long? NextExecution { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("requestTimeout")]
    public int RequestTimeout { get; set; }

    [JsonPropertyName("redirectSuccess")]
    public bool RedirectSuccess { get; set; }

    [JsonPropertyName("folderId")]
    public int FolderId { get; set; }

    [JsonPropertyName("schedule")]
    public Schedule Schedule { get; set; }

    [JsonPropertyName("requestMethod")]
    public int RequestMethod { get; set; }
}