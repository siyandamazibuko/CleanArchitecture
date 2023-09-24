using CleanArchitecture.Common.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = CleanArchitecture.Common.Exceptions.ValidationException;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<TRequest> _logger;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {            
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var validationResponse = new ValidationFailureResponse { Errors = new Dictionary<string, string[]>() };
                foreach (var failure in failures)
                {
                    validationResponse.Errors.Add(failure.PropertyName, new string[] { failure.ErrorMessage });
                }

                var requestName = typeof(TRequest).Name;

                throw new ValidationException(validationResponse);
            }

            return next();
        }
    }
}
