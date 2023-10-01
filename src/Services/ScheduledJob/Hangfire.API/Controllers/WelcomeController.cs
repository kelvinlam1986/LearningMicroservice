using Contracts.ScheduledJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Hangfire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        private readonly IScheduledJobService _jobService;
        private readonly Serilog.ILogger _logger;

        public WelcomeController(
            IScheduledJobService jobService,
            Serilog.ILogger logger) 
        { 
            _jobService = jobService;
            _logger = logger;   
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Welcome()
        {
            var jobId = _jobService.Enqueue(() => ResponseWelcome("Welcome"));
            return Ok($"JobId {jobId}. Enqueue Job");   
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult DelayWelcome()
        {
            var seconds = 5;
            var jobId = _jobService.Schedule(() => ResponseWelcome("Welcome"), TimeSpan.FromSeconds(seconds));
            return Ok($"JobId {jobId}. Delay Job After {seconds} seconds");
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult WelcomeAt()
        {
            var enqueueAt = DateTimeOffset.UtcNow.AddSeconds(5);
            var jobId = _jobService.Schedule(() => ResponseWelcome("Welcome"), enqueueAt);
            return Ok($"JobId {jobId}. Delay Job At {enqueueAt} seconds");
        }

        [NonAction]
        public void ResponseWelcome(string text)
        {
            _logger.Information(text);
        }
    }
}
