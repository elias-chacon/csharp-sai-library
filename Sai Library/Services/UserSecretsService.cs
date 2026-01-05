using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class UserSecretsService : BaseService
    {
        public UserSecretsService(IHttpClientBase httpClient, string baseUrl,
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetSecrets()
        {
            return Get("/api/secrets");
        }

        public Result<JsonNode> CreateSecret(string variable, string secret)
        {
            var data = new Dictionary<string, object>
            {
                ["variable"] = variable,
                ["secret"] = secret
            };
            return Post("/api/secrets", data);
        }

        public Result<JsonNode> UpdateSecret(string id, string variable, string secret)
        {
            var data = new Dictionary<string, object>
            {
                ["variable"] = variable,
                ["secret"] = secret
            };
            return Put($"/api/secrets/{id}", data);
        }

        public Result<JsonNode> DeleteSecret(string id)
        {
            return Delete($"/api/secrets/{id}");
        }
    }
}