using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services.Interfaces;
using Shared.Services.Email;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IScheduledJobService _scheduledJobService;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly ILogger _logger;

        public BackgroundJobService(IScheduledJobService scheduledJobService, ISmtpEmailService smtpEmailService, ILogger logger)
        {
            _scheduledJobService = scheduledJobService;
            _smtpEmailService = smtpEmailService;
            _logger = logger;
        }

        public IScheduledJobService ScheduledJobService => _scheduledJobService;

        public string SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueuedAt)
        {
            var emailRequest = new MailRequest
            {
                ToAddress = email,
                Subject = subject,
                Body = emailContent,
            };

            try
            {
                var jobId = _scheduledJobService.Schedule(() => _smtpEmailService.SendEmail(emailRequest), enqueuedAt);
                _logger.Information($"Sent email to {email} with subject {subject}. JobId = {jobId}");
                return jobId;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed due to an error in email service. Error {ex.Message}" );
                throw ex;
            }
        }
    }
}
