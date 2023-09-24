using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CleanArchitecture.Common.Security;
using Serilog;
using NR = NewRelic.Api.Agent;

namespace CleanArchitecture.Common.Agents
{
    public abstract class ServiceAgentBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _clientName;
        private readonly ISecurityServiceAgent _securityServiceAgent;

        protected ServiceAgentBase(IHttpClientFactory clientFactory, ISecurityServiceAgent securityServiceAgent, string httpClientName)
        {
            _clientName = httpClientName;
            _clientFactory = clientFactory;
            _securityServiceAgent = securityServiceAgent;
        }
        
        [NR.Trace]
        protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, int timeout = 10, bool useBearToken = true)
        {
            var client = _clientFactory.CreateClient(_clientName);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            if (useBearToken)
            {
                var tokenResponse = await _securityServiceAgent.GetClientCredentialsTokenAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse);
            }

            return await SendAsync(client, requestMessage);
        }

        [NR.Trace]
        protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, string serviceAccount, string serviceAccountPassword, int timeout = 10)
        {
            var client = _clientFactory.CreateClient(_clientName);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            var authenticationBytes = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{serviceAccount}:{serviceAccountPassword}"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", authenticationBytes);

            return await SendAsync(client, requestMessage);
        }

        // [NR.Trace]
        // protected async Task<HttpResponseMessage> SendWithDelegationAsync(HttpRequestMessage requestMessage, ISecurityOptions securityOptions, string token, int timeout = 10)
        // {
        //     var delegationTokenRequest = new DelegationTokenRequest()
        //     {
        //         Token = token,
        //         DelegationClientId = securityOptions.DelegationClientId,
        //         DelegationClientSecret = securityOptions.DelegationClientSecret,
        //         Scopes = securityOptions.Scopes,
        //         Authority = securityOptions.Authority
        //     };
        //
        //     var delegationToken = await SecurityServiceAgent.GetDelegationTokenAsync(delegationTokenRequest);
        //
        //     var client = ClientFactory.CreateClient(ClientName);
        //     client.Timeout = TimeSpan.FromMinutes(timeout);
        //     requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", delegationToken);
        //
        //     return await SendAsync(client, requestMessage);
        // }
        
        [NR.Trace]
        protected static async Task<T> GetResponseAsync<T>(HttpResponseMessage response)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<T>(resultString);
            return a;
        }

        [NR.Trace]
        protected string AppendApiVersion(string path, string apiVersion)
        {
            if (!string.IsNullOrEmpty(apiVersion))
                path += path.Contains("?") ? $"&api-version={apiVersion}" : $"?api-version={apiVersion}";

            return path;
        }

        [NR.Trace]
        private async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage requestMessage)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await client.SendAsync(requestMessage);
            }
            catch (Exception ex)
            {
                HttpClientFailureCustomEvent(ex, requestMessage, response);
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                var exception = new Exception($"ResponseStatusCode: {response.StatusCode}. Errors: {error}");

                HttpClientFailureCustomEvent(exception, requestMessage, response);

                throw exception;
            }

            return response;
        }
        private void HttpClientFailureCustomEvent(Exception exception, HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            if (exception.Message.Contains("AdditionalData for Client"))
                return;

            var responseStatusCode = responseMessage?.StatusCode ?? HttpStatusCode.BadRequest;
            Log.Error(exception, $"{_clientName} API HTTP request failure with status code {responseStatusCode}. Errors: {exception.Message}");
            NR.NewRelic.RecordCustomEvent("FX:HttpClientRequestFailure", new[]
             {
                   new KeyValuePair<string, object>("Service", _clientName),
                   new KeyValuePair<string, object>("Error", exception.Message),
                   new KeyValuePair<string, object>("StatusCode", responseStatusCode),
                   new KeyValuePair<string, object>("RequestUri", requestMessage.RequestUri),
                   new KeyValuePair<string, object>("RequestHeaders", requestMessage.Headers),
                   new KeyValuePair<string, object>("RequestContent", requestMessage.Content)

            });
        }
    }
}
