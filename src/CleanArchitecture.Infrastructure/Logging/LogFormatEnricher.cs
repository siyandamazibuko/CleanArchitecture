using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace CleanArchitecture.Infrastructure.Logging
{
    public class LogFormatEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string[] properties = { "env", "machineId", "taskResult", "threadId" };
            
            var agent = new
            {
                podname = logEvent.Properties.ContainsKey("machineId") ? logEvent.Properties["machineId"].ToString() : "not found"
            };

            foreach (var property in properties)
            {
                logEvent.RemovePropertyIfPresent(property);
            }

            var log = new
            {
                level = logEvent.Level
            };

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("@version", Assembly.GetEntryAssembly()?.GetName().Version));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("agent", agent,true));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("log",log,true));
        }
    }
}
