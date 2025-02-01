using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CronJob.Org.Sdk.Models.CronJob;
using CronJob.Org.Sdk.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



namespace CronJob.Org.Sdk.Services.Providers;

public class CronJobService : ICronJobService
{
    private readonly HttpClient _httpClient;
    private readonly string _cronJobApiUrl;
    private readonly string _bearerToken;
    private readonly ILogger<CronJobService> _logger;

    public CronJobService(IConfiguration configuration, ILogger<CronJobService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = new HttpClient();

        try
        {
            _cronJobApiUrl = configuration["CronJobApi:BaseUrl"] 
                ?? throw new ArgumentNullException("CronJobApi:BaseUrl missing in configuration");
            _bearerToken = configuration["CronJobApi:BearerToken"] 
                ?? throw new ArgumentNullException("CronJobApi:BearerToken missing in configuration");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger.LogInformation("CronJobService initialized successfully with API URL: {ApiUrl}", _cronJobApiUrl);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize CronJobService");
            throw;
        }
    }
    
    
    public async Task<bool> DeleteJobAsync(long jobId)
    {
        try
        {
            _logger.LogDebug("Deleting job ID: {JobId}", jobId);

            // Clear and set headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.DeleteAsync($"{_cronJobApiUrl}/{jobId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response - Status: {StatusCode}, Content: {ResponseContent}", 
                response.StatusCode, responseContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted job ID: {JobId}", jobId);
                return true;
            }

            _logger.LogError("Failed to delete job. Status: {StatusCode}, Response: {Response}", 
                response.StatusCode, responseContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting job ID: {JobId}", jobId);
            throw;
        }
    }
    
    public async Task<GetJobsResponse> GetAllJobsAsync()
    {
        try
        {
            _logger.LogDebug("Fetching all cron jobs");

            // Clear and set headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync($"{_cronJobApiUrl}"); // Note: endpoint is /jobs
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response - Status: {StatusCode}, Content: {ResponseContent}", 
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch jobs. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseContent);
                throw new HttpRequestException($"Failed to fetch jobs: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var result = JsonSerializer.Deserialize<GetJobsResponse>(responseContent, options);
            _logger.LogInformation("Successfully fetched {JobCount} jobs", result.Jobs.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching cron jobs");
            throw;
        }
    }
    
    public async Task<JobDetailsResponse> GetJobDetailsAsync(long jobId)
    {
        try
        {
            _logger.LogDebug("Fetching details for job ID: {JobId}", jobId);

            // Clear and set headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync($"{_cronJobApiUrl}/{jobId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response - Status: {StatusCode}, Content: {ResponseContent}", 
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch job details. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, responseContent);
                throw new HttpRequestException($"Failed to fetch job details: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var result = JsonSerializer.Deserialize<JobDetailsResponse>(responseContent, options);
            _logger.LogInformation("Successfully fetched details for job ID: {JobId}", jobId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching job details for ID: {JobId}", jobId);
            throw;
        }
    }

    public async Task<ScheduleJobResponse> ScheduleJobAsync(CronJobScheduleRequest request)
    {
        try
        {
            _logger.LogInformation("Processing job schedule request for webhook: {WebhookUrl}", request.WebhookUrl);
            
            var validateResponse = ValidateRequest(request);

            if (!validateResponse.Success)
            {
                return validateResponse;
            }
            var apiRequest = CreateApiRequest(request);
            
            _logger.LogDebug("Created API request for scheduling. Schedule type: {ScheduleType}", request.ScheduleType);
            
            var response = await SendScheduleRequestAsync(apiRequest);
            
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            var result = new ScheduleJobResponse 
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode
            };

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<CronJobApiResponse>(responseContent);
                result.JobId = apiResponse?.JobId;
                result.Message = $"Job scheduled successfully with ID: {apiResponse?.JobId}";
            }
            else 
            {
                result.Message = $"Failed to schedule job: {response.StatusCode}. Response: {responseContent}";
            }

            if (result.Success)
            {
                _logger.LogInformation("Successfully scheduled job for webhook: {WebhookUrl}", request.WebhookUrl);
            }
            else
            {
                _logger.LogError("Failed to schedule job. Status code: {StatusCode}, Webhook: {WebhookUrl}", 
                    response.StatusCode, request.WebhookUrl);
            }
            
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Validation failed for job schedule request. Webhook: {WebhookUrl}", request.WebhookUrl);
            return new ScheduleJobResponse
            {
                Success = false,
                Message = $"Validation error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while scheduling job. Webhook: {WebhookUrl}", request.WebhookUrl);
            return new ScheduleJobResponse
            {
                Success = false,
                Message = $"An unexpected error occurred: {ex.Message}"
            };
        }
    }

    private ScheduleJobResponse ValidateRequest(CronJobScheduleRequest request)
    {
        _logger.LogDebug("Validating schedule request");

        try
        {
            if (string.IsNullOrWhiteSpace(request.WebhookUrl))
            {
                return new ScheduleJobResponse 
                { 
                    Success = false, 
                    Message = "WebhookUrl is required" 
                };
            }

            if (request.ScheduleType == JobScheduleType.Recurring)
            {
                if (request.Hour.HasValue && (request.Hour.Value < 0 || request.Hour.Value > 23))
                {
                    return new ScheduleJobResponse 
                    { 
                        Success = false, 
                        Message = "Hour must be between 0 and 23" 
                    };
                }

                if (request.Minute.HasValue && (request.Minute.Value < 0 || request.Minute.Value > 59))
                {
                    return new ScheduleJobResponse 
                    { 
                        Success = false, 
                        Message = "Minute must be between 0 and 59" 
                    };
                }
            }
            else if (!request.ExecutionTime.HasValue)
            {
                return new ScheduleJobResponse 
                { 
                    Success = false, 
                    Message = "ExecutionTime is required for one-time jobs" 
                };
            }

            _logger.LogDebug("Request validation successful");
            return new ScheduleJobResponse { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during validation");
            return new ScheduleJobResponse 
            { 
                Success = false, 
                Message = $"Validation error: {ex.Message}" 
            };
        }
    }

    private CronJobApiRequest CreateApiRequest(CronJobScheduleRequest request)
    {
        try
        {
            _logger.LogDebug("Creating API request for schedule type: {ScheduleType}", request.ScheduleType);

            var schedule = new Schedule
            {
                Timezone = request.TimeZone,
                ExceptionDates = new List<int>()
            };

            if (request.ScheduleType == JobScheduleType.Recurring)
            {
                var recurringResult = ConfigureRecurringSchedule(schedule, request);
                if (!recurringResult.Success)
                {
                    return null;
                }
            }
            else
            {
                var oneTimeResult = ConfigureOneTimeSchedule(schedule, request);
                if (!oneTimeResult.Success)
                {
                    return null;
                }
            }

            _logger.LogDebug("API request created successfully");

            return new CronJobApiRequest
            {
                Job = new Job
                {
                    Url = request.WebhookUrl,
                    Enabled = true,
                    SaveResponses = true,
                    Schedule = schedule
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create API request");
            return null;
        }
    }

   private ScheduleJobResponse ConfigureRecurringSchedule(Schedule schedule, CronJobScheduleRequest request)
{
    try
    {
        schedule.ExpiresAt = 0;

        switch (request.RecurringPattern)
        {
            case RecurringPattern.EveryMinute:
                if (request.MinuteInterval.HasValue)
                {
                    var minutes = new List<int>();
                    for (int i = 0; i < 60; i += request.MinuteInterval.Value)
                    {
                        minutes.Add(i);
                    }
                    schedule.Hours = new List<int> { -1 };
                    schedule.Minutes = minutes;
                }
                else
                {
                    schedule.Hours = new List<int> { -1 };
                    schedule.Minutes = new List<int> { -1 };
                }
                schedule.Mdays = new List<int> { -1 };
                schedule.Months = new List<int> { -1 };
                schedule.Wdays = new List<int> { -1 };
                break;

            case RecurringPattern.Hourly:
                schedule.Hours = new List<int> { -1 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { 0 };
                schedule.Mdays = new List<int> { -1 };
                schedule.Months = new List<int> { -1 };
                schedule.Wdays = new List<int> { -1 };
                break;

            case RecurringPattern.Daily:
                schedule.Hours = request.Hour.HasValue 
                    ? new List<int> { request.Hour.Value }
                    : new List<int> { 0 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { 0 };
                schedule.Mdays = new List<int> { -1 };
                schedule.Months = new List<int> { -1 };
                schedule.Wdays = new List<int> { -1 };
                break;

            case RecurringPattern.EveryXDays:
                if (!request.DayInterval.HasValue || request.DayInterval.Value < 1)
                {
                    return new ScheduleJobResponse 
                    { 
                        Success = false, 
                        Message = "DayInterval must be specified and greater than 0 for EveryXDays pattern" 
                    };
                }

                var currentDate = DateTime.Now;
                var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                var days = new List<int>();
                
                for (int day = 1; day <= daysInMonth; day += request.DayInterval.Value)
                {
                    days.Add(day);
                }

                schedule.Hours = request.Hour.HasValue 
                    ? new List<int> { request.Hour.Value }
                    : new List<int> { 0 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { 0 };
                schedule.Mdays = days;
                schedule.Months = new List<int> { -1 };
                schedule.Wdays = new List<int> { -1 };
                break;

            case RecurringPattern.Weekly:
                if (!request.DayOfWeek.HasValue)
                {
                    return new ScheduleJobResponse 
                    { 
                        Success = false, 
                        Message = "DayOfWeek is required for weekly pattern" 
                    };
                }
                schedule.Hours = request.Hour.HasValue 
                    ? new List<int> { request.Hour.Value }
                    : new List<int> { 0 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { 0 };
                schedule.Mdays = new List<int> { -1 };
                schedule.Months = new List<int> { -1 };
                schedule.Wdays = new List<int> { (int)request.DayOfWeek.Value };
                break;

            case RecurringPattern.Monthly:
                if (!request.DayOfMonth.HasValue)
                {
                    return new ScheduleJobResponse 
                    { 
                        Success = false, 
                        Message = "DayOfMonth is required for monthly pattern" 
                    };
                }
                schedule.Hours = request.Hour.HasValue 
                    ? new List<int> { request.Hour.Value }
                    : new List<int> { 0 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { 0 };
                schedule.Mdays = new List<int> { request.DayOfMonth.Value };
                
                if (request.MonthInterval.HasValue && request.MonthInterval.Value > 1)
                {
                    var months = new List<int>();
                    for (int i = 1; i <= 12; i += request.MonthInterval.Value)
                    {
                        months.Add(i);
                    }
                    schedule.Months = months;
                }
                else
                {
                    schedule.Months = new List<int> { -1 };
                }
                
                schedule.Wdays = new List<int> { -1 };
                break;

            case RecurringPattern.Yearly:
                if (!request.Month.HasValue || !request.DayOfMonth.HasValue)
                {
                    return new ScheduleJobResponse 
                    { 
                        Success = false, 
                        Message = "Month and DayOfMonth are required for yearly pattern" 
                    };
                }
                schedule.Hours = request.Hour.HasValue 
                    ? new List<int> { request.Hour.Value }
                    : new List<int> { 0 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { 0 };
                schedule.Mdays = new List<int> { request.DayOfMonth.Value };
                schedule.Months = new List<int> { request.Month.Value };
                schedule.Wdays = new List<int> { -1 };
                break;

            case RecurringPattern.Custom:
                schedule.Hours = request.Hour.HasValue 
                    ? new List<int> { request.Hour.Value }
                    : new List<int> { -1 };
                schedule.Minutes = request.Minute.HasValue 
                    ? new List<int> { request.Minute.Value }
                    : new List<int> { -1 };
                schedule.Mdays = request.DayOfMonth.HasValue 
                    ? new List<int> { request.DayOfMonth.Value }
                    : new List<int> { -1 };
                schedule.Months = request.Month.HasValue 
                    ? new List<int> { request.Month.Value }
                    : new List<int> { -1 };
                schedule.Wdays = request.DayOfWeek.HasValue 
                    ? new List<int> { (int)request.DayOfWeek.Value }
                    : new List<int> { -1 };
                break;
        }

        return new ScheduleJobResponse { Success = true };
    }
    catch (Exception ex)
    {
        return new ScheduleJobResponse 
        { 
            Success = false, 
            Message = $"Failed to configure recurring schedule: {ex.Message}" 
        };
    }
}
    private ScheduleJobResponse ConfigureOneTimeSchedule(Schedule schedule, CronJobScheduleRequest request)
    {
        try
        {
            _logger.LogDebug("Configuring one-time schedule for execution time: {ExecutionTime}", 
                request.ExecutionTime);

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            var executionTimeInZone = TimeZoneInfo.ConvertTime(
                request.ExecutionTime.Value,
                TimeZoneInfo.Utc,
                timeZoneInfo
            );

            schedule.Hours = new List<int> { executionTimeInZone.Hour };
            schedule.Minutes = new List<int> { executionTimeInZone.Minute };
            schedule.Mdays = new List<int> { executionTimeInZone.Day };
            schedule.Months = new List<int> { executionTimeInZone.Month };
            schedule.Wdays = new List<int> { (int)executionTimeInZone.DayOfWeek };

            var expirationTime = executionTimeInZone.AddMinutes(1);
            schedule.ExpiresAt = long.Parse(expirationTime.ToString("yyyyMMddHHmmss"));

            return new ScheduleJobResponse { Success = true };
        }
        catch (Exception ex)
        {
            return new ScheduleJobResponse 
            { 
                Success = false, 
                Message = $"Failed to configure one-time schedule: {ex.Message}" 
            };
        }
    }

    private async Task<HttpResponseMessage> SendScheduleRequestAsync(CronJobApiRequest request)
    {
        try
        {
            _logger.LogDebug("Preparing to send schedule request to API");

            // Clear and set headers explicitly
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _bearerToken
            );
            _httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var requestJson = JsonSerializer.Serialize(request, options);
            _logger.LogDebug("Request payload: {RequestJson}", requestJson);

            // Create and configure content
            using var content = new StringContent(
                requestJson,
                Encoding.UTF8,
                "application/json"
            );
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PutAsync(_cronJobApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
        
            _logger.LogDebug("API Response - Status: {StatusCode}, Content: {ResponseContent}", 
                response.StatusCode, responseContent);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send schedule request to API");
            throw;
        }
    }
    
    
}