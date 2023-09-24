using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace CleanArchitecture.Common.Extensions
{
    public static class HttpExtensions
    {
        public static IAsyncPolicy<HttpResponseMessage> RetryPolicy(string prefix, int retries = 3)
        {
            var random = new Random();
            var retryWithJitterPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                // .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retries,
                    // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))  
                                    + TimeSpan.FromMilliseconds(random.Next(0, 100)),
                    (response, timeSpan, retry, ctx) =>
                    {
                        var exception = response.Exception;

                        if (response.Exception != null)
                        {
                            Log.Warning(
                                "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                                prefix, exception.GetType().Name, exception.Message, retry, retries);
                        }
                        else if (response.Result != null)
                        {
                            Log.Warning("[{prefix}] Status {statusCode} detected retry {retry} of {retries}",
                                prefix, response.Result.StatusCode.ToString(), retry, retries);                            
                        }                        
                    }
                );

            return retryWithJitterPolicy;
        }
        
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
