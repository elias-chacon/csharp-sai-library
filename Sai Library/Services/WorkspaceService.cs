using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class WorkspaceService : BaseService
    {
        public WorkspaceService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetWorkspaces(int resultSize = 20, int page = 1)
        {
            return Get("/api/workspaces", new Dictionary<string, object>
            {
                ["ResultSize"] = resultSize,
                ["Page"] = page
            });
        }

        public Result<JsonNode> GetWorkspace(string id)
        {
            return Get($"/api/workspaces/{id}");
        }

        public Result<JsonNode> AddTemplateToWorkspace(string workspaceId, 
            string templateId)
        {
            return Post($"/api/workspaces/{workspaceId}/templates/{templateId}", null);
        }

        public Result<JsonNode> RemoveTemplateFromWorkspace(string workspaceId, 
            string templateId)
        {
            return Delete($"/api/workspaces/{workspaceId}/templates/{templateId}");
        }
    }
}