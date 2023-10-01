using Contracts.Services;
using Shared.Services.Email;
using Serilog;
using Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading;

namespace Infrastructure.Services
{
    public class SmtpEmailService : ISmtpEmailService
    {
        private readonly ILogger _logger;
        private readonly SMTPEmailSettings _emailSettings;
        private readonly SmtpClient _smtpClient;

        public SmtpEmailService(
            ILogger logger,
            SMTPEmailSettings emailSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
            _smtpClient = new SmtpClient();
        }

        public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = default)
        {
            var emailMessage = new MimeMessage
            {
                Sender = new MailboxAddress(_emailSettings.DisplayName, request.From ?? _emailSettings.From),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.Body
                }.ToMessageBody()
            };

            if (request.ToAddresses.Any())
            {
                foreach (var toAddress in request.ToAddresses) 
                {
                    emailMessage.To.Add(MailboxAddress.Parse(toAddress));
                }
            }
            else
            {
                var toAddress = request.ToAddress;
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }

            try
            {
                await _smtpClient.ConnectAsync(
                    _emailSettings.SMTPServer,
                    _emailSettings.Port,
                    cancellationToken: cancellationToken);

                await _smtpClient.AuthenticateAsync(
                    _emailSettings.Username, _emailSettings.Password, cancellationToken);
                await _smtpClient.SendAsync(emailMessage, cancellationToken);
                await _smtpClient.DisconnectAsync(true, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void SendEmail(MailRequest request)
        {
            var emailMessage = new MimeMessage
            {
                Sender = new MailboxAddress(_emailSettings.DisplayName, request.From ?? _emailSettings.From),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.Body
                }.ToMessageBody()
            };

            if (request.ToAddresses.Any())
            {
                foreach (var toAddress in request.ToAddresses)
                {
                    emailMessage.To.Add(MailboxAddress.Parse(toAddress));
                }
            }
            else
            {
                var toAddress = request.ToAddress;
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }

            try
            {
                _smtpClient.Connect(_emailSettings.SMTPServer, _emailSettings.Port);
                _smtpClient.Authenticate(_emailSettings.Username, _emailSettings.Password);
                _smtpClient.Send(emailMessage);
                _smtpClient.Disconnect(true);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
