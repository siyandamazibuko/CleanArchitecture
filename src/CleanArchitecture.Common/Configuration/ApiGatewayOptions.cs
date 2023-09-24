namespace CleanArchitecture.Common.Configuration
{
    public class ApiGatewayOptions : OptionsBase<ApiGatewayOptions>
    {
        public string BaseUrl { set; get; }
        public int RetryCount { get; set; }
        public string ApiVersion { set; get; }
        public string ApiKey { get; set; }
    }
}
