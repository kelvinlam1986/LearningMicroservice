using Hangfire.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.ScheduledJobs;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.API.Controllers
{
    [Route("api/scheduled-jobs")]
    [ApiController]
    public class ScheduledJobsController : ControllerBase
    {
        private readonly IBackgroundJobService _jobService;

        public ScheduledJobsController(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost]
        [Route("send-email-checkout-reminder-order")]
        public async Task<ActionResult> SendReminderCheckoutEmail([FromBody] ReminderCheckoutOrderDto model)
        {
            var jobId = _jobService.SendEmailContent(model.Email, model.Subject, model.EmailContent, model.EnqueueAt);
            return Ok(jobId);
        }

        [HttpDelete]
        [Route("delete/jobId/{id}")]
        public IActionResult DeleteJobId([Required] string id)
        {
            var result = _jobService.ScheduledJobService.Delete(id);
            return Ok(result);
        }
    }
}
