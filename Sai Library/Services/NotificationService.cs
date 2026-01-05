using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class NotificationService : BaseService
    {
        public NotificationService(IHttpClientBase httpClient, string baseUrl,
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetNotifications()
        {
            return Get("/api/notifications");
        }

        public Result<JsonNode> MarkNotificationAsRead(string id)
        {
            return Patch($"/api/notifications/{id}/read", null);
        }

        public Result<JsonNode> MarkAllNotificationsAsRead()
        {
            return Patch("/api/notifications/mark-all-read", null);
        }

        public Result<JsonNode> DismissNotification(string id)
        {
            return Patch($"/api/notifications/{id}/dismiss", null);
        }

        public Result<JsonNode> GetUnreadCount()
        {
            return Get("/api/notifications/count/unread");
        }
    }
}