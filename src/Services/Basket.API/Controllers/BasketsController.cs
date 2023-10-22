using AutoMapper;
using Basket.API.DTO;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using EventBus.Message.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTO.Baskets;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        
        public BasketsController(
            IBasketRepository repository,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        [HttpGet("{username}", Name = "GetBasket")]
        public async Task<ActionResult<CartDto>> GetCartByUsername([Required] string username)
        {
            var cart = await _repository.GetBasketByUsername(username);
            var result = _mapper.Map<CartDto>(cart);

            return Ok(result == null ? new CartDto() : result);
        }

        [HttpPost(Name = "UpdateBasket")]
        public async Task<ActionResult<CartDto>> UpdateCart([FromBody] CartDto model)
        {
            //foreach (var item in cart.Items)
            //{
            //    var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
            //    item.SetAvailableQuantity(stock.Quantity);
            //}

            var options = new DistributedCacheEntryOptions 
            { 
                AbsoluteExpiration = DateTime.UtcNow.AddDays(1),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            var cart = _mapper.Map<Cart>(model);
            var updatedCart = await _repository.UpdateBasket(cart, options);
            var result = _mapper.Map<CartDto>(updatedCart);

            return Ok(result);
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        public async Task<IActionResult> DeleteBasket([Required] string username)
        {
            var result = await _repository.DeleteBasketFromUsername(username);
            return Ok(result);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Checkout([FromBody] Basket.API.DTO.BasketCheckoutDto baseketCheckout)
        {
            var basket = await _repository.GetBasketByUsername(baseketCheckout.UserName);
            if (basket == null)
            {
                return NotFound();
            }

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(baseketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);

            await _repository.DeleteBasketFromUsername(baseketCheckout.UserName);
            return Accepted();
        }
    }
}
