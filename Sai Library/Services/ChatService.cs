using System.Text.Json.Nodes;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class ChatService : BaseService
    {
        public ChatService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> SendCompletion(List<Dictionary<string, object>> messages, 
            string model, Dictionary<string, object> options = null)
        {
            var data = new Dictionary<string, object>
            {
                ["messages"] = messages,
                ["model"] = model,
                ["temperature"] = options != null && options.ContainsKey("temperature") 
                    ? options["temperature"] 
                    : 0.7,
                ["max_tokens"] = options != null && options.ContainsKey("max_tokens") 
                    ? options["max_tokens"] 
                    : 1000
            };

            if (options != null && options.ContainsKey("seed"))
            {
                data["seed"] = options["seed"];
            }

            return Post("/api/prompt/v1/chat/completions", data);
        }
    }
}