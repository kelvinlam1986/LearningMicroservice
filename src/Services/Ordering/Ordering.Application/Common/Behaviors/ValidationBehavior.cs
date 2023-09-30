using FluentValidation;
using MediatR;

namespace Ordering.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);
            var validationResult = await Task.WhenAll(
                _validators.Select(x => x.ValidateAsync(context, cancellationToken)));

            var failures = validationResult
                .Where(x => x.Errors.Any())
                .SelectMany(x => x.Errors)
                .ToList();

            if (failures.Any())
            {
                throw new Common.Exceptions.ValidationException(failures);
            }

            return await next();
        }
    }
}
