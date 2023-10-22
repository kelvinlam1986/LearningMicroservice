using MediatR;

namespace Ordering.Application.Features.V1.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest
    {
        public DeleteOrderCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
