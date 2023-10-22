using AutoMapper;
using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocumentNo;
using Ordering.Application.Features.V1.Orders.Queries.GetOrderById;
using Ordering.Application.Features.V1.Orders.Queries.GetOrders;
using Shared.DTO.Order;
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
        private readonly IMapper _mapper;

        public OrdersController(
            IMediator mediator,
            ISmtpEmailService emailService,
            IMessageProducer messageProducer,
            IMapper mapper)
        {
            _mediator = mediator;
            _emailService = emailService;
            _messageProducer = messageProducer;
            _mapper = mapper;
        }

        private static class RoutesName
        {
            public const string GetOrders = nameof(GetOrders);
            public const string GetOrder = nameof(GetOrder);
            public const string CreateOrder = nameof(CreateOrder);
            public const string DeleteOrder = nameof(DeleteOrder);
            public const string DeleteOrderByDocumentNo = nameof(DeleteOrderByDocumentNo);  
        }

        [HttpGet("{username}", Name = RoutesName.GetOrders)]
        [ProducesResponseType(typeof(IEnumerable<Ordering.Application.Common.Models.OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Ordering.Application.Common.Models.OrderDto>>> GetOrdersByUserName([Required] string username)
        {
            var query = new GetOrdersQuery(username);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:long}", Name = RoutesName.GetOrder)]
        [ProducesResponseType(typeof(Ordering.Application.Common.Models.OrderDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Ordering.Application.Common.Models.OrderDto>> GetOrderById([Required] long id)
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost(Name = RoutesName.CreateOrder)]
        [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderDto model)
        {
            var command = _mapper.Map<CreateOrderCommand>(model);
            var result = await _mediator.Send(command);
           // _messageProducer.SendMessage(result.Data);
            return result;

        }

        [HttpDelete("{id:long}", Name = RoutesName.DeleteOrder)]
        [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> DeleteOrder([Required] long id)
        {
            var command = new DeleteOrderCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("document-no/{documentNo}", Name = RoutesName.DeleteOrderByDocumentNo)]
        [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.NoContent)] 
        public async Task<ApiResult<bool>> DeleteOrderByDocumentNo([Required] string documentNo)
        {
            var command = new DeleteOrderByDocumentNoCommand(documentNo);
            var result = await _mediator.Send(command);
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
