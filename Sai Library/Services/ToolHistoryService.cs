using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class ToolHistoryService : BaseService
    {
        public ToolHistoryService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetToolHistory(string templateId = null, 
            List<string> templateIds = null, string workspaceId = null)
        {
            var query = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(templateId))
                query["templateId"] = templateId;
                
            if (templateIds != null && templateIds.Any())
                query["templateIds"] = templateIds;
                
            if (!string.IsNullOrEmpty(workspaceId))
                query["workspaceId"] = workspaceId;

            return Get("/api/tool-history", query);
        }

        public Result<JsonNode> CreateToolHistory(string templateId, 
            Dictionary<string, object> inputs, 
            List<Dictionary<string, object>> chatMessages = null, 
            string workspaceId = null)
        {
            var data = new Dictionary<string, object>
            {
                ["templateId"] = templateId,
                ["inputs"] = inputs ?? new Dictionary<string, object>(),
                ["chatMessages"] = chatMessages ?? new List<Dictionary<string, object>>()
            };

            if (!string.IsNullOrEmpty(workspaceId))
                data["workspaceId"] = workspaceId;

            return Post("/api/tool-history", data);
        }

        public Result<JsonNode> GetToolHistoryItem(string id, string workspaceId = null)
        {
            var query = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(workspaceId))
                query["workspaceId"] = workspaceId;

            return Get($"/api/tool-history/{id}", query);
        }

        public Result<JsonNode> DeleteToolHistoryItem(string id)
        {
            return Delete($"/api/tool-history/{id}");
        }

        public Result<JsonNode> SearchToolHistory(string query, 
            string workspaceId = null, int resultSize = 20)
        {
            return Get("/api/tool-history/search", new Dictionary<string, object>
            {
                ["Query"] = query,
                ["WorkspaceId"] = workspaceId,
                ["ResultSize"] = resultSize
            });
        }

        public Result<JsonNode> RestoreToolHistory(string id, string workspaceId = null)
        {
            var data = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(workspaceId))
                data["workspaceId"] = workspaceId;

            return Post($"/api/tool-history/{id}/restore", data);
        }
    }
}