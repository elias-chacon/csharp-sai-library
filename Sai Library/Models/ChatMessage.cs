namespace Sai_Library.Models
{
    public class ChatMessage
    {
        public string Role { get; }
        public List<Dictionary<string, object>> Content { get; }

        public ChatMessage(string role, string text)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Content = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["type"] = "text",
                    ["text"] = text
                }
            };
        }

        public ChatMessage(string role, List<Dictionary<string, object>> content)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Content = new List<Dictionary<string, object>>(content);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                ["role"] = Role,
                ["content"] = Content
            };
        }

        public static ChatMessage CreateWithImage(string role, string text,
            string imageUrl, string detail = "auto")
        {
            var content = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["type"] = "text",
                    ["text"] = text
                },
                new Dictionary<string, object>
                {
                    ["type"] = "image_url",
                    ["image_url"] = new Dictionary<string, object>
                    {
                        ["url"] = imageUrl,
                        ["detail"] = detail
                    }
                }
            };

            return new ChatMessage(role, content);
        }
    }
}