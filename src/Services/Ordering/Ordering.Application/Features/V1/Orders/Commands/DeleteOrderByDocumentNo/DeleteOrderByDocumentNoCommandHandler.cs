using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Intefaces;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrder;
using Shared.SeedWork;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocumentNo
{
    public class DeleteOrderByDocumentNoCommandHandler : IRequestHandler<DeleteOrderByDocumentNoCommand, ApiResult<bool>>
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger _logger;

        public DeleteOrderByDocumentNoCommandHandler(
            IOrderRepository repository,
            ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResult<bool>> Handle(DeleteOrderByDocumentNoCommand request, CancellationToken cancellationToken)
        {
            _logger.Information($"BEGIN {nameof(DeleteOrderByDocumentNoCommandHandler)} DocumentNo={request.DocumentNo}");

            var order = await _repository.GetOrderByDocumentNo(request.DocumentNo);
            if (order == null)
            {
                throw new NotFoundException($"{nameof(order)}", request.DocumentNo);
            }

            await _repository.DeleteAsync(order);
            await _repository.SaveChangesAsync();

            _logger.Information($"END {nameof(DeleteOrderByDocumentNoCommandHandler)} Id={request.DocumentNo}");

            return new ApiResult<bool>(true, true, "Delete Successfully");
        }
    }
}
