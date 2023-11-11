using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTO.ScheduledJobs;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly BackgroundJobHttpService _backgroundJobHttpService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger _logger;

        public BasketRepository(
            IDistributedCache redisCacheService,
            ISerializeService serializeService,
            BackgroundJobHttpService backgroundJobHttpService,
            IEmailTemplateService emailTemplateService,
            ILogger logger)
        {
            _redisCacheService = redisCacheService;
            _serializeService = serializeService;
            _backgroundJobHttpService = backgroundJobHttpService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
        }

        public async Task<bool> DeleteBasketFromUsername(string username)
        {
            await DeleteReminderCheckoutOrder(username);
            try
            {
                await _redisCacheService.RemoveAsync(username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public async Task<Cart?> GetBasketByUsername(string username)
        {
            _logger.Information($"BEGIN GetBasketByUsername username={username}");
            var basket = await _redisCacheService.GetStringAsync(username);
            _logger.Information($"END GetBasketByUsername username={username}");
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            await DeleteReminderCheckoutOrder(cart.UserName);
            if (options != null)
            {
                await _redisCacheService.SetStringAsync(cart.UserName,
                    _serializeService.Serialize(cart), options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
            }

            try
            {
                await TriggerSendEmailReminderCheckout(cart);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return await GetBasketByUsername(cart.UserName);
        }

        private async Task TriggerSendEmailReminderCheckout(Cart cart)
        {
            var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail(cart.UserName);
            var model = new ReminderCheckoutOrderDto(cart.EmailAddress, "Reminder Checkout", emailTemplate, DateTimeOffset.UtcNow.AddSeconds(30));

            var uri = $"{_backgroundJobHttpService.ScheduledJobUrl}/send-email-checkout-reminder-order";
            var response = await _backgroundJobHttpService.Client.PostAsJson<ReminderCheckoutOrderDto>(uri, model);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                var jobId = await response.ReadContentAs<string>();
                if (string.IsNullOrEmpty(jobId) == false)
                {
                    cart.JobId = jobId;
                    await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
                }
            }
        }

        private async Task DeleteReminderCheckoutOrder(string username)
        {
            var cart = await GetBasketByUsername(username);
            if (cart == null || string.IsNullOrEmpty(cart.JobId))
            {
                return;
            }

            var jobId = cart.JobId;
            var uri = $"{_backgroundJobHttpService.ScheduledJobUrl}/delete/jobId/{jobId}";
            await _backgroundJobHttpService.Client.DeleteAsync(uri);
            _logger.Information($"DeleteReminderCheckoutOrder. Deleted JobId {jobId}");
        }
    }
}
