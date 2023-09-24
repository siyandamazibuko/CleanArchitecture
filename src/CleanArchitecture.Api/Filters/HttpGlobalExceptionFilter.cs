using CleanArchitecture.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using CleanArchitecture.Common.Guards;

namespace CleanArchitecture.Api.Filters
{
    public delegate IActionResult CustomExceptionHandler(Exception exception);

    public class HttpGlobalExceptionFilter
        : IExceptionFilter
    {
        #region Class Fields

        readonly IWebHostEnvironment _environment;
        readonly ILogger _logger;
        readonly CustomExceptionHandler _customExceptionHandler;

        #endregion Class Fields

        #region Constructors

        public HttpGlobalExceptionFilter(
        IWebHostEnvironment environment,
            ILogger<HttpGlobalExceptionFilter> logger,
            CustomExceptionHandler customExceptionHandler = null)
        {
            _environment = Check.NotNull(environment, nameof(environment));
            _logger = Check.NotNull(logger, nameof(logger));
            _customExceptionHandler = customExceptionHandler;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            IActionResult result = null;

            if (_customExceptionHandler != null)
                result = _customExceptionHandler(exception);

            if (result == null)
            {
                switch (exception)
                {
                    case KeyNotFoundException keyNotFoundException:
                        {
                            _logger.LogWarning($"Requested resource {keyNotFoundException.Message} not found");
                            result = new NotFoundResult();
                        }
                        break;
                    case UnauthorizedAccessException unauthorizedAccessException:
                        {
                            _logger.LogWarning("The request did not include a valid authentication token");
                            result = new UnauthorizedResult();
                        }
                        break;
                    case ValidationException validationException:
                        {
                            var errors = validationException.Errors?.Select(e => string.Join(", ", e.Value.Select(v => $"[{v}]").ToArray()));
                            if (errors == null)
                            {
                                _logger.LogWarning("The request triggered one or more validation failures");
                            }
                            else
                            {
                                var failures = string.Join(" ", errors);
                                _logger.LogWarning($"Validation Failure(s): {failures}");
                            }

                            result = new BadRequestObjectResult(new ValidationFailureResponse { Title = validationException.Message, Errors = validationException.Errors });
                        }
                        break;
                    case OperationCanceledException operationCanceledException:
                        {
                            _logger.LogError("The request did not complete in the permitted time");

                            // Gateway timeout
                            object json = ResolveErrorMessage(context.Exception);
                            result = new ServerErrorObjectResult(json, HttpStatusCode.GatewayTimeout);
                        }
                        break;
                    default:
                        {
                            _logger.LogCritical(context.Exception, "An unexpected exception has occurred!");

                            // Unhandled/unexpected
                            object json = ResolveErrorMessage(context.Exception);
                            result = new ServerErrorObjectResult(json, HttpStatusCode.InternalServerError);
                        }
                        break;
                }
            }
            context.Result = result;
            context.ExceptionHandled = true;
        }

        object ResolveErrorMessage(Exception exception)
        {
            if (_environment.IsDevelopment())
                return new { Messages = new[] { "An unexpected error has occurred" }, DeveloperMessage = exception };

            return new { Messages = new[] { "An unexpected error has occurred" } };
        }

        #endregion Public Methods
    }
}
