# CronJob.Org SDK for .NET

A .NET SDK for scheduling and managing cron jobs using the cron-job.org API. This SDK provides a simple interface to create, manage, and monitor scheduled tasks.

## Features

- Schedule one-time and recurring jobs
- Flexible recurring patterns (minutes, hourly, daily, weekly, monthly, yearly)
- Support for intervals (every X minutes, days, or months)
- Manage existing cron jobs
- Retrieve job details and status
- Delete scheduled jobs
- Supports multiple time zones
- Built-in request validation
- Comprehensive logging

## Installation

Install the package via NuGet:

```bash
dotnet add package CronJobOrgSdk
```

## Configuration

Add the following to your `appsettings.json`:

```json
{
  "CronJobApi": {
    "BaseUrl": "YOUR_API_BASE_URL",
    "BearerToken": "YOUR_API_TOKEN"
  }
}
```

Register the service in your `Program.cs` or `Startup.cs`:

```csharp
builder.Services.AddCronJobOrgSdk();
```

## Usage

### Schedule a One-time Job

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.OneTime,
    ExecutionTime = DateTime.UtcNow.AddHours(1),
    TimeZone = "America/New_York"
};

var response = await _cronJobService.ScheduleJobAsync(request);
```

### Schedule Recurring Jobs

#### Every Minute

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.EveryMinute,
    TimeZone = "UTC"
};
```

#### Every X Minutes

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.EveryMinute,
    MinuteInterval = 15,    // Run every 15 minutes
    TimeZone = "UTC"
};
```

#### Hourly (Every hour at specific minute)

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Hourly,
    Minute = 30,        // Run at minute 30 of every hour
    TimeZone = "UTC"
};
```

#### Daily Job

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Daily,
    Hour = 14,          // Run at 2 PM
    Minute = 30,        // at 30 minutes
    TimeZone = "UTC"
};
```

#### Every X Days

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.EveryXDays,
    DayInterval = 2,    // Run every 2 days
    Hour = 9,          // at 9 AM
    Minute = 0,
    TimeZone = "UTC"
};
```

#### Weekly Job

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Weekly,
    DayOfWeek = DayOfWeek.Monday,  // Run every Monday
    Hour = 10,
    Minute = 0,
    TimeZone = "UTC"
};
```

#### Monthly Job

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Monthly,
    DayOfMonth = 15,   // Run on 15th of every month
    Hour = 14,
    Minute = 30,
    TimeZone = "UTC"
};
```

#### Every X Months (e.g., Quarterly)

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Monthly,
    DayOfMonth = 1,    // Run on 1st of every 3rd month
    MonthInterval = 3, // Every 3 months
    Hour = 9,
    Minute = 0,
    TimeZone = "UTC"
};
```

#### Yearly Job

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Yearly,
    Month = 3,         // March
    DayOfMonth = 15,   // 15th
    Hour = 15,
    Minute = 0,
    TimeZone = "UTC"
};
```

#### Custom Pattern

```csharp
var request = new CronJobScheduleRequest
{
    WebhookUrl = "https://api.example.com/webhook",
    ScheduleType = JobScheduleType.Recurring,
    RecurringPattern = RecurringPattern.Custom,
    Month = 6,         // June
    DayOfMonth = 1,    // 1st
    DayOfWeek = DayOfWeek.Monday, // Only on Mondays
    Hour = 12,
    Minute = 30,
    TimeZone = "UTC"
};
```

### Get Job Details

```csharp
var jobDetails = await _cronJobService.GetJobDetailsAsync(jobId);
Console.WriteLine($"Job URL: {jobDetails.JobDetails.Url}");
```

### Get All Jobs

```csharp
var allJobs = await _cronJobService.GetAllJobsAsync();
foreach (var job in allJobs.Jobs)
{
    Console.WriteLine($"Job ID: {job.JobId}, Status: {job.Status}");
}
```

### Delete a Job

```csharp
var deleted = await _cronJobService.DeleteJobAsync(jobId);
if (deleted)
{
    Console.WriteLine("Job deleted successfully");
}
```

## Request Models

### CronJobScheduleRequest

```csharp
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
    public int? MonthInterval { get; set; }
    public int? DayInterval { get; set; }
}

public enum RecurringPattern
{
    EveryMinute,
    Hourly,
    Daily,
    EveryXDays,
    Weekly,
    Monthly,
    Yearly,
    Custom
}

public enum JobScheduleType
{
    OneTime,
    Recurring
}
```

## Response Models

### ScheduleJobResponse

```csharp
public class ScheduleJobResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public long? JobId { get; set; }
}
```

### GetJobsResponse

```csharp
public class GetJobsResponse
{
    [JsonPropertyName("jobs")]
    public List<JobDetails> Jobs { get; set; }

    [JsonPropertyName("someFailed")]
    public bool SomeFailed { get; set; }
}
```

### JobDetailsResponse

```csharp
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
```

### JobDetails

```csharp
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
```

### Schedule Configuration

```csharp
public class Schedule
{
    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("expiresAt")]
    public long ExpiresAt { get; set; }

    [JsonPropertyName("hours")]
    public List<int> Hours { get; set; }

    [JsonPropertyName("minutes")]
    public List<int> Minutes { get; set; }

    [JsonPropertyName("mdays")]
    public List<int> Mdays { get; set; }

    [JsonPropertyName("months")]
    public List<int> Months { get; set; }

    [JsonPropertyName("wdays")]
    public List<int> Wdays { get; set; }

    [JsonPropertyName("exceptionDates")]
    public List<int> ExceptionDates { get; set; }
}
```

## Error Handling

The SDK includes comprehensive error handling and logging. All methods throw appropriate exceptions for invalid requests or API errors. It's recommended to wrap calls in try-catch blocks:

```csharp
try
{
    var response = await _cronJobService.ScheduleJobAsync(request);
    // Handle success
}
catch (ArgumentException ex)
{
    // Handle validation errors
    _logger.LogError(ex, "Invalid request");
}
catch (HttpRequestException ex)
{
    // Handle API communication errors
    _logger.LogError(ex, "API error");
}
catch (Exception ex)
{
    // Handle unexpected errors
    _logger.LogError(ex, "Unexpected error");
}
```

## Logging

The SDK uses Microsoft.Extensions.Logging for comprehensive logging of operations. Configure logging in your application to capture:

- API requests and responses
- Validation errors
- Configuration issues
- Runtime errors

## Supported .NET Versions

- .NET 6.0
- .NET 7.0
- .NET 8.0

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
