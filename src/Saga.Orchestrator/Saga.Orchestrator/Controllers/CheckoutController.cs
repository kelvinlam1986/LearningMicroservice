using Contracts.Sagas.OrderManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTO.Baskets;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _sagaOrderManager;

        public CheckoutController(ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOrderManager)
        {
            _sagaOrderManager = sagaOrderManager ?? throw new ArgumentNullException(nameof(sagaOrderManager));
        }

        [HttpPost]
        [Route("{username}")]
        public OrderResponse CheckoutOrder([Required] string username,
            [FromBody] BasketCheckoutDto model)
        {
            model.UserName = username;
            
            var result = _sagaOrderManager.CreateOrder(model);
            return result;
        }
    }
}
