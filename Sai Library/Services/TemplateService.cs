using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class TemplateService : BaseService
    {
        public TemplateService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetTemplates(Dictionary<string, object> filters = null)
        {
            return Get("/api/templates/list", filters);
        }

        public Result<JsonNode> GetTemplate(string id)
        {
            return Get($"/api/templates/{id}");
        }

        public Result<JsonNode> GetTemplateView(string id)
        {
            return Get($"/api/templates/{id}/view");
        }

        public Result<JsonNode> ExecuteTemplate(string id, 
            Dictionary<string, object> inputs, 
            Dictionary<string, object> options = null)
        {
            var queryParams = new Dictionary<string, object>();
            
            if (options != null)
            {
                if (options.TryGetValue("workspaceId", out var workspaceId))
                    queryParams["workspaceId"] = workspaceId;
                if (options.TryGetValue("seed", out var seed))
                    queryParams["seed"] = seed;
                if (options.TryGetValue("modelOverride", out var modelOverride))
                    queryParams["modelOverride"] = modelOverride;
            }

            var data = new Dictionary<string, object>
            {
                ["inputs"] = inputs ?? new Dictionary<string, object>()
            };

            if (options != null && options.TryGetValue("chatMessages", out var chatMessages))
                data["chatMessages"] = chatMessages;

            if (options != null && options.TryGetValue("secrets", out var secrets))
                data["secrets"] = secrets;

            return Post($"/api/templates/{id}/execute", data, queryParams);
        }

        public Result<JsonNode> ExecuteChatTemplate(string id, 
            List<Dictionary<string, object>> messages, 
            string workspaceId = null)
        {
            var queryParams = workspaceId != null 
                ? new Dictionary<string, object> { ["workspaceId"] = workspaceId } 
                : null;

            var data = new Dictionary<string, object>
            {
                ["messages"] = messages
            };

            return Post($"/api/templates/{id}/chatexecute", data, queryParams);
        }

        public Result<JsonNode> GetSubscribedTemplates()
        {
            return Get("/api/templates/subscribed");
        }

        public Result<JsonNode> GetOwnedTemplates()
        {
            return Get("/api/templates/owned");
        }

        public Result<JsonNode> SubscribeToTemplate(string id)
        {
            return Put($"/api/templates/subscribe/{id}", null);
        }

        public Result<JsonNode> UnsubscribeFromTemplate(string id)
        {
            return Put($"/api/templates/unsubscribe/{id}", null);
        }
    }
}