using CronJob.Org.Sdk.Models.CronJob;
using CronJob.Org.Sdk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CronJobTestEnvironmentApi.Controllers;

[ApiController]
[Route("api/cron-job")]
public class CronJobController : ControllerBase
{
    
    private readonly ICronJobService _cronJobService;

    public CronJobController( ICronJobService cronJobService)
    {
     
        _cronJobService = cronJobService;

        
    }

   /// <summary>
   /// TEST schedule
   /// </summary>
   /// <returns></returns>
    [HttpPost("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TestCronJob()
    {
        try
        {
            
            // var request = new CronJobScheduleRequest
            // {
            //     WebhookUrl = "https://webhook.site/d820d15e-7f9c-4a91-b723-e19114f3ce3f",
            //     ScheduleType = JobScheduleType.Recurring,
            //     Hour = 14,          // Run at 2 PM
            //     Minute = 30,        // at 30 minutes
            //     TimeZone = "UTC"
            // };
            
            // var request = new CronJobScheduleRequest
            // {
            //     WebhookUrl = "https://webhook.site/d820d15e-7f9c-4a91-b723-e19114f3ce3f",
            //     ScheduleType = JobScheduleType.OneTime,
            //     ExecutionTime = DateTime.UtcNow.AddHours(1),
            //     TimeZone = "America/New_York"
            // };

            var request = new CronJobScheduleRequest
            {
                WebhookUrl = "https://webhook.site/8a8fb52d-b56f-4e79-8813-f091b7041efb",
                ScheduleType = JobScheduleType.Recurring,
                MinuteInterval = 15,    // Run every 15 minutes
                TimeZone = "UTC"
            };

          
            var response = await _cronJobService.ScheduleJobAsync(request);
            return Ok();
            
            
        }
        catch (Exception ex)
        {
           
            return BadRequest();
        }
    }
   
   
    /// <summary>
    /// Get all jobs
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCronJobs()
    {
        try
        {
            
            var response = await _cronJobService.GetAllJobsAsync();
            return Ok();
        }
        catch (Exception ex)
        {
           
            return BadRequest();
        }
    }
    
    
  /// <summary>
  /// Get job by id
  /// </summary>
  /// <param name="jobId"></param>
  /// <returns></returns>
    [HttpGet("{jobId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCronJobById([FromRoute] long jobId)
    {
        try
        {
            
            var response = await _cronJobService.GetJobDetailsAsync(jobId);
            return Ok();
        }
        catch (Exception ex)
        {
           
            return BadRequest();
        }
    }
  
  
    /// <summary>
    /// Get job by id
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    [HttpDelete("{jobId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCronJobById([FromRoute] long jobId)
    {
        try
        {
            
            var response = await _cronJobService.DeleteJobAsync(jobId);
            return Ok();
        }
        catch (Exception ex)
        {
           
            return BadRequest();
        }
    }
}