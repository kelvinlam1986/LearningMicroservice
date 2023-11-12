using Infrastructure.Extensions;
using Shared.Configurations;
using Shared.DTO.ScheduledJobs;

namespace Basket.API.Services
{
    public class BackgroundJobHttpService
    {
        private readonly HttpClient _client;

        private readonly string _scheduledJobUrl;

        public BackgroundJobHttpService(
            HttpClient client,
            BackgroundJobSettings settings)
        {
            client.BaseAddress = new Uri(settings.HangfireUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
            _scheduledJobUrl = settings.ScheduledJobUrl;
        }

        public async Task<string> SendEmailReminderCheckout(ReminderCheckoutOrderDto model)
        {
            var uri = $"{_scheduledJobUrl}/send-email-checkout-reminder-order";
            var response = await _client.PostAsJson<ReminderCheckoutOrderDto>(uri, model);
            string jobId = null;
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                jobId = await response.Content.ReadAsStringAsync();
            }

            return jobId;
        }

        public async Task DeleteReminderCheckoutOrder(string jobId)
        {
            var uri = $"{_scheduledJobUrl}/delete/jobId/{jobId}";
            await _client.DeleteAsync(uri);
        }
    }
}
