using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cib.Markets.Core.Common.Helpers;
using Cib.Markets.Core.Context;
using Cib.Markets.Core.Correlation.Context;

namespace CleanArchitecture.Api.Http
{
    public sealed class CorrelationIdHandler : DelegatingHandler
    {
        private const string Header = "X-Correlation-ID";

        private readonly IContextAccessor<CorrelationContext> _contextAccessor;

        public CorrelationIdHandler(IContextAccessor<CorrelationContext> contextAccessor)
        {
            _contextAccessor = Guard.IsNotNull(contextAccessor, "contextAccessor");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Header))
            {
                var correlationId = _contextAccessor.Context?.CorrelationId;
                if (correlationId.HasValue)
                {
                    request.Headers.Add(Header, correlationId.Value.ToString());
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
