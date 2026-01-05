using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class CategoryService : BaseService
    {
        public CategoryService(IHttpClientBase httpClient, string baseUrl,
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetCategories(List<int> types = null,
            string parentCategoryKeyName = null, string name = null)
        {
            var query = new Dictionary<string, object>();

            if (types != null && types.Any())
                query["types"] = types;

            if (!string.IsNullOrEmpty(parentCategoryKeyName))
                query["parentCategoryKeyName"] = parentCategoryKeyName;

            if (!string.IsNullOrEmpty(name))
                query["name"] = name;

            return Get("/api/category", query);
        }

        public Result<JsonNode> GetCategoryTypes()
        {
            return Get("/api/category/types");
        }

        public Result<JsonNode> GetCategory(string id)
        {
            return Get($"/api/category/{id}");
        }
    }
}