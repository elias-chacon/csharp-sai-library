using System.Text.Json;
using System.Text.Json.Nodes;
using Sai_Library.Enums;
using Sai_Library.Http;
using Sai_Library.Models;
using UriBuilder = Sai_Library.Utils.UriBuilder;

namespace Sai_Library.Services
{
    public abstract class BaseService
    {
        protected readonly IHttpClientBase HttpClient;
        protected readonly string BaseUrl;
        protected readonly Dictionary<string, string> Headers;
        protected readonly JsonSerializerOptions JsonOptions;

        protected BaseService(IHttpClientBase httpClient, string baseUrl,
            Dictionary<string, string>? headers)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            BaseUrl = (baseUrl ?? throw new ArgumentNullException(nameof(baseUrl))).TrimEnd('/');
            Headers = headers != null
                ? new Dictionary<string, string>(headers)
                : new Dictionary<string, string>();

            JsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected Result<JsonNode> Get(string endpoint, Dictionary<string, object> queryParams = null)
        {
            var uri = UriBuilder.Build(BaseUrl, endpoint, queryParams);
            return HttpClient.MakeRequest(uri, RequestMethod.GET, Headers);
        }

        protected Result<JsonNode> Post(string endpoint, object data,
            Dictionary<string, object> queryParams = null)
        {
            var uri = UriBuilder.Build(BaseUrl, endpoint, queryParams);
            var body = ToJson(data);
            return HttpClient.MakeRequest(uri, RequestMethod.POST, Headers, body);
        }

        protected Result<JsonNode> Put(string endpoint, object data,
            Dictionary<string, object> queryParams = null)
        {
            var uri = UriBuilder.Build(BaseUrl, endpoint, queryParams);
            var body = ToJson(data);
            return HttpClient.MakeRequest(uri, RequestMethod.PUT, Headers, body);
        }

        protected Result<JsonNode> Patch(string endpoint, object data,
            Dictionary<string, object> queryParams = null)
        {
            var uri = UriBuilder.Build(BaseUrl, endpoint, queryParams);
            var body = ToJson(data);
            return HttpClient.MakeRequest(uri, RequestMethod.PATCH, Headers, body);
        }

        protected Result<JsonNode> Delete(string endpoint,
            Dictionary<string, object> queryParams = null)
        {
            var uri = UriBuilder.Build(BaseUrl, endpoint, queryParams);
            return HttpClient.MakeRequest(uri, RequestMethod.DELETE, Headers);
        }

        protected string ToJson(object data)
        {
            if (data == null) return null;
            return JsonSerializer.Serialize(data, JsonOptions);
        }
    }
}