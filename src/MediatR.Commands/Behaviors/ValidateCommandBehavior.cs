namespace MediatR.Commands
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class ValidateCommandBehavior<TRequest, TResponse> : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public ValidateCommandBehavior(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override async Task<TResponse> Process(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // validate only if implements interface
            if (!(request is IValidateCommand command))
            {
                return await next().ConfigureAwait(false);
            }

            var validationResult = command.Validate();
            if (!validationResult.IsValid)
            {
                throw new ValidationException($"{command.GetType().Name} has validation errors: " + validationResult.Errors.Safe().Select(e => $"{e.PropertyName}={e}").ToString(", "), validationResult.Errors);
            }

            return await next().ConfigureAwait(false); // continue pipeline if no validation errors
        }
    }
}
