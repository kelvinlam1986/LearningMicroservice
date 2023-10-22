using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTO.Baskets;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
        }

        [HttpPost]
        [Route("{username}")]
        public async Task<IActionResult> CheckoutOrder([Required] string username,
            [FromBody] BasketCheckoutDto model)
        {
            var result = await _checkoutService.CheckoutOrder(username, model);
            return Accepted(result);
        }
    }
}
