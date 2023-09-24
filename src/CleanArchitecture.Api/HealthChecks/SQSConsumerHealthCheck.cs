using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitecture.Api.HealthChecks
{
    public class SQSConsumerHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var last_Sync_Time = Globals.SQS_LAST_SYNC_TIME;
            var utcNow = DateTime.UtcNow;
            TimeSpan ts = utcNow - last_Sync_Time;

            if (ts.TotalMilliseconds > (1000 * 60))
            {
                return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy));
            }

            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
        }
    }
}
