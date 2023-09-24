using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CleanArchitecture.Api.Logging;
using Serilog;

namespace CleanArchitecture.Api.Filters
{
    public class ActionLogFilterAttribute : ActionFilterAttribute
    {
        const string TimedOperationKey = "ActionTimer";

        const string ActionExecutingMessageTemplate = "Executing {controller}.{action}";
        const string ActionExecutedMessageTemplate = "{controller}.{action} finished in {elapsed} ms";

        private readonly ILogger _logger;

        public ActionLogFilterAttribute(ILogger logger)
        {
            this._logger = logger.ForContext<ActionLogFilterAttribute>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items[TimedOperationKey] = Stopwatch.StartNew();

            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];

            this._logger.ForContext(LogProperties.Pointcut, Pointcut.Inbound)
                .ForContext("actionArgs", context.ActionArguments, true)
                .Information(ActionExecutingMessageTemplate, controllerName, actionName);

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            var stopwatch = (Stopwatch)context.HttpContext.Items[TimedOperationKey];
            stopwatch.Stop();

            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];

            if (context.Exception != null)
            {
                this._logger.ForContext(LogProperties.Pointcut, Pointcut.Error)
                    .Error(context.Exception, ActionExecutedMessageTemplate, controllerName, actionName, stopwatch.ElapsedMilliseconds.ToString());
            }
            else
            {
                ILogger logger = this._logger.ForContext(LogProperties.Pointcut, Pointcut.Outbound);

                if (context.Result is ObjectResult objectResult)
                {
                    logger = logger.ForContext("actionResult", objectResult.Value, true);
                }

                logger.Information(ActionExecutedMessageTemplate, controllerName, actionName, stopwatch.ElapsedMilliseconds.ToString());
            }
        }
    }
}
