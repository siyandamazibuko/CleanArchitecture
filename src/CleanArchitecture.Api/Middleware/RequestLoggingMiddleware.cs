using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using CleanArchitecture.Api.Logging;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace CleanArchitecture.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        const string RequestMethod = "requestMethod";
        const string RequestPath = "requestPath";
        const string StatusCode = "statusCode";
        const string Elapsed = "elapsed";

        const string RequestMessageTemplate = "HTTP {requestMethod} {requestPath}";
        const string ResponseMessageTemplate = "HTTP {requestMethod} {requestPath} responded {statusCode} in {elapsed} ms";
        const LogEventLevel LogLevel = LogEventLevel.Information;

        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IEnumerable<ILogEventEnricher> enrichers)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var stopwatch = Stopwatch.StartNew();
            var logContext = LogContext.Push(enrichers.ToArray());

            var defaultProperties = new LogEventProperty[]
            {
                new LogEventProperty(RequestMethod, new ScalarValue(httpContext.Request.Method)),
                new LogEventProperty(RequestPath, new ScalarValue(GetPath(httpContext)))
            };

            var properties = new LogEventProperty[]
            {
                new LogEventProperty(LogProperties.Pointcut, new ScalarValue(Pointcut.Inbound))
            };

            this.Log(RequestMessageTemplate, defaultProperties.Concat(properties));

            HttpStatusCode statusCode = HttpStatusCode.OK;
            var pointcut = Pointcut.Outbound;
            Exception exception = null;

            try
            {
                await _next(httpContext);

                statusCode = (HttpStatusCode)httpContext.Response.StatusCode;
            }
            catch (Exception e)
            {
                statusCode = HttpStatusCode.InternalServerError;
                pointcut = Pointcut.Error;
                exception = e;

                throw;
            }
            finally
            {
                stopwatch.Stop();

                properties = new LogEventProperty[]
                {
                    new LogEventProperty(LogProperties.Pointcut, new ScalarValue(pointcut)),
                    new LogEventProperty(StatusCode, new ScalarValue(statusCode)),
                    new LogEventProperty(Elapsed, new ScalarValue(stopwatch.ElapsedMilliseconds.ToString()))
                };

                this.Log(ResponseMessageTemplate, defaultProperties.Concat(properties), exception);

                logContext.Dispose();
            }
        }

        private void Log(string messageTemplate, IEnumerable<LogEventProperty> properties, Exception exception = null)
        {
            var logger = Serilog.Log.ForContext<RequestLoggingMiddleware>();

            if (!logger.IsEnabled(LogLevel))
            {
                return;
            }

            var mt = new MessageTemplateParser().Parse(messageTemplate);

            var logEvent = new LogEvent(DateTimeOffset.Now, LogLevel, exception, mt, properties);
            logger.Write(logEvent);
        }

        static string GetPath(HttpContext httpContext)
        {
            var requestPath = httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;
            if (string.IsNullOrEmpty(requestPath))
            {
                requestPath = httpContext.Request.Path.ToString();
            }

            return requestPath;
        }
    }
}
