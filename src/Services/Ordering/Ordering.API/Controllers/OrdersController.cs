using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Application.Features.V1.Orders.Queries.GetOrders;
using Shared.SeedWork;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISmtpEmailService _emailService;
        private readonly IMessageProducer _messageProducer;
        
        public OrdersController(
            IMediator mediator,
            ISmtpEmailService emailService,
            IMessageProducer messageProducer)
        {
            _mediator = mediator;
            _emailService = emailService;
            _messageProducer = messageProducer;
        }

        private static class RoutesName
        {
            public const string GetOrders = nameof(GetOrders);
            public const string CreateOrder = nameof(CreateOrder);
        }

        [HttpGet("{username}", Name = RoutesName.GetOrders)]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
        {
            var query = new GetOrdersQuery(username);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost(Name = RoutesName.CreateOrder)]
        [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            _messageProducer.SendMessage(result.Data);
            return result;

        }

        [HttpGet("test-email")]
        public async Task<ActionResult> TestEmail()
        {
            var message = new MailRequest
            {
                Body = "<h1>Hello</h1>",
                Subject = "Test",
                ToAddress = "kelvinlam_1986@yahoo.com.vn"
            };

            await _emailService.SendEmailAsync(message);
            return Ok();
        }
    }
}
