using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace CleanArchitecture.Common.Agents
{
    public abstract class ServiceAgentBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _clientName;

        protected ServiceAgentBase(IHttpClientFactory clientFactory, string httpClientName)
        {
            _clientName = httpClientName;
            _clientFactory = clientFactory;
        }
        
        protected static async Task<T> GetResponseAsync<T>(HttpResponseMessage response)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var a = JsonConvert.DeserializeObject<T>(resultString);
            return a;
        }

        protected string AppendApiVersion(string path, string apiVersion)
        {
            if (!string.IsNullOrEmpty(apiVersion))
                path += path.Contains("?") ? $"&api-version={apiVersion}" : $"?api-version={apiVersion}";

            return path;
        }

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
        }
    }
}
