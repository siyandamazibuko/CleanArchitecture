using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Api.Logging.Destructurers;
using Serilog.Core;
using Serilog.Events;

namespace CleanArchitecture.Api.Logging.Enrichers
{
    public class ExceptionEnricher : ILogEventEnricher
    {
        private const string ExceptionDetail = "exception.detail";
        private const string DestructurerNotFoundMessage = "Destructurer not found for '{0}'.";

        private readonly Dictionary<Type, IExceptionDestructurer> _destructurers;

        public ExceptionEnricher(IEnumerable<IExceptionDestructurer> destructurers)
        {
            this._destructurers = destructurers.ToDictionary(d => d.GetTargetType());
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception != null)
            {
                var targetType = logEvent.Exception.GetType();

                if (this._destructurers.ContainsKey(targetType))
                {
                    var d = this._destructurers[targetType];
                    var t = d.Handle(logEvent.Exception);

                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(ExceptionDetail, t, true));
                }
                else
                {
                    var detail = string.Format(DestructurerNotFoundMessage, logEvent.Exception.GetType().Name);

                    logEvent.AddPropertyIfAbsent(new LogEventProperty(ExceptionDetail, new ScalarValue(detail)));
                }

                logEvent.AddOrUpdateProperty(new LogEventProperty(LogProperties.Pointcut, new ScalarValue(Pointcut.Error)));
            }
        }
    }
}
