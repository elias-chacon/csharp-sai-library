using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Sai_Library.Enums;
using Sai_Library.Models;

namespace Sai_Library.Http
{
    public class SystemNetHttpClient : IHttpClientBase
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public int TimeoutSeconds { get; set; } = 30;

        public SystemNetHttpClient()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(TimeoutSeconds)
            };
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public Result<JsonNode> MakeRequest(string uri, RequestMethod method, 
            Dictionary<string, string> headers = null, string body = null)
        {
            try
            {
                return MakeRequestAsync(uri, method, headers, body).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return Result<JsonNode>.Error($"HTTP Request failed: {ex.Message}");
            }
        }

        private async Task<Result<JsonNode>> MakeRequestAsync(string uri, RequestMethod method,
            Dictionary<string, string> headers, string body)
        {
            using var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = GetHttpMethod(method)
            };

            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var status = (int)response.StatusCode;

            if (status >= 200 && status < 300)
            {
                JsonNode json = string.IsNullOrWhiteSpace(responseBody)
                    ? new JsonObject()
                    : JsonNode.Parse(responseBody);
                
                return Result<JsonNode>.Success(json, new Dictionary<string, object>
                {
                    ["status"] = status
                });
            }

            return Result<JsonNode>.Error($"HTTP {status}: {responseBody}");
        }

        private static HttpMethod GetHttpMethod(RequestMethod method)
        {
            return method switch
            {
                RequestMethod.GET => HttpMethod.Get,
                RequestMethod.POST => HttpMethod.Post,
                RequestMethod.PUT => HttpMethod.Put,
                RequestMethod.PATCH => HttpMethod.Patch,
                RequestMethod.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get
            };
        }
    }
}