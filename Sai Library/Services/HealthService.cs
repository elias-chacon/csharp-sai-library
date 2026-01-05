using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class HealthService : BaseService
    {
        public HealthService(IHttpClientBase httpClient, string baseUrl,
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> CheckHealth()
        {
            return Get("/api/hc");
        }
    }
}