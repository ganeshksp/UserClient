namespace UserClient.Infrastructure
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "https://reqres.in/api";
        public string ApiKey { get; set; } = string.Empty;
        public string ApiKeyHeaderName { get; set; } = "x-api-key";
    }
}
