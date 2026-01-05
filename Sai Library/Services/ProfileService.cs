using System.Net;
using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class ProfileService : BaseService
    {
        public ProfileService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetConfig()
        {
            return Get("/api/profile/config");
        }

        public Result<JsonNode> GetLanguage()
        {
            return Get("/api/profile/language");
        }

        public Result<JsonNode> SetLanguage(string language)
        {
            return Put($"/api/profile/language/{WebUtility.UrlEncode(language)}", null);
        }

        public Result<JsonNode> GetName()
        {
            return Get("/api/profile/name");
        }

        public Result<JsonNode> GetEmail()
        {
            return Get("/api/profile/email");
        }

        public Result<JsonNode> IsDeveloper()
        {
            return Get("/api/profile/developer");
        }

        public Result<JsonNode> IsAdmin()
        {
            return Get("/api/profile/admin");
        }
    }
}