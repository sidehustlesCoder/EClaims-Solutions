using eClaims.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace eClaims.Infrastructure.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            // In a real app, uses SMTP or SendGrid.
            // Here we simulate by logging.
            _logger.LogInformation($"[NOTIFICATION] Sending Email to {to}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body: {body}");
            
            await Task.CompletedTask;
        }
    }
}
