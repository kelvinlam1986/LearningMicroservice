using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Intefaces;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger _logger;

        public DeleteOrderCommandHandler(
            IOrderRepository repository,
            ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.Information($"BEGIN {nameof(DeleteOrderCommandHandler)} Id={request.Id}");

            var order = await _repository.GetByIdAsync(request.Id);
            if (order == null) 
            {
                throw new NotFoundException($"{nameof(order)}", request.Id);
            }

            await _repository.DeleteAsync(order);
            await _repository.SaveChangesAsync();

            _logger.Information($"END {nameof(DeleteOrderCommandHandler)} Id={request.Id}");

            return Unit.Value;
        }
    }
}
