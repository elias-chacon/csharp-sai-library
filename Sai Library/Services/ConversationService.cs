using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class ConversationService : BaseService
    {
        public ConversationService(IHttpClientBase httpClient, string baseUrl,
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> CreateConversation(string title,
            string templateId = null, string workspaceId = null)
        {
            var data = new Dictionary<string, object>
            {
                ["title"] = title
            };

            if (!string.IsNullOrEmpty(templateId))
                data["templateId"] = templateId;

            if (!string.IsNullOrEmpty(workspaceId))
                data["workspaceId"] = workspaceId;

            return Post("/api/conversations", data);
        }

        public Result<JsonNode> GetConversations(string templateId = null,
            string workspaceId = null)
        {
            var query = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(templateId))
                query["templateId"] = templateId;

            if (!string.IsNullOrEmpty(workspaceId))
                query["workspaceId"] = workspaceId;

            return Get("/api/conversations", query);
        }

        public Result<JsonNode> GetConversation(string conversationId,
            string workspaceId = null)
        {
            var query = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(workspaceId))
                query["workspaceId"] = workspaceId;

            return Get($"/api/conversations/{conversationId}", query);
        }

        public Result<JsonNode> DeleteConversation(string conversationId)
        {
            return Delete($"/api/conversations/{conversationId}");
        }

        public Result<JsonNode> UpdateConversationTitle(string conversationId,
            string title)
        {
            var data = new Dictionary<string, object> { ["title"] = title };
            return Put($"/api/conversations/{conversationId}/title", data);
        }
    }
}