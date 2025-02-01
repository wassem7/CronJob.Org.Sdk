using System.Text.Json.Serialization;

namespace CronJob.Org.Sdk.Models.CronJob;

public class CronJobScheduleRequest
{
    public string WebhookUrl { get; set; }
    public JobScheduleType ScheduleType { get; set; }
    public string TimeZone { get; set; } = "UTC";

    // For one-time jobs
    public DateTime? ExecutionTime { get; set; }

    // For recurring jobs
    public RecurringPattern RecurringPattern { get; set; }
    public int? Hour { get; set; }
    public int? Minute { get; set; }
    public int? DayOfMonth { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? Month { get; set; }
    public int? MinuteInterval { get; set; }
    public int? MonthInterval { get; set; } // For every X months
    public int? DayInterval { get; set; }   // For every X days
}
