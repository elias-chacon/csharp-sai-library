using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using Sai_Library.Enums;
using Sai_Library.Http;
using Sai_Library.Models;
using Sai_Library.Services;
using Sai_Library.Utils;

namespace Sai_Library
{
    public class SAILibrary
    {
        private HealthService _health;
        private ProfileService _profile;
        private ModelService _models;
        private ChatService _chat;
        private TemplateService _templates;
        private ConversationService _conversations;
        private WorkspaceService _workspaces;
        private ToolHistoryService _toolHistory;
        private CategoryService _categories;
        private FileService _files;
        private UserSecretsService _userSecrets;
        private NotificationService _notifications;
        private readonly IHttpClientBase _httpClient;
        private string _selectedModel;
        private List<JsonNode>? _availableModels = new List<JsonNode>();

        private SAILibrary(string apiKey, string baseUrl, IHttpClientBase httpClient)
        {
            baseUrl = NormalizeBaseUrl(CheckForBaseUrl(baseUrl));
            apiKey = CheckForApiKey(apiKey);
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var headers = CreateDefaultHeaders(apiKey);
            InitializeServices(headers, baseUrl, _httpClient);
            LoadModels();
        }

        private static string NormalizeBaseUrl(string baseUrl)
        {
            return baseUrl.TrimEnd('/');
        }

        private static Dictionary<string, string>? CreateDefaultHeaders(string apiKey)
        {
            return new Dictionary<string, string>
            {
                ["X-Api-Key"] = apiKey,
                ["Content-Type"] = "application/json"
            };
        }

        private void InitializeServices(Dictionary<string, string>? headers,
            string baseUrl, IHttpClientBase client)
        {
            _health = new HealthService(client, baseUrl, headers);
            _profile = new ProfileService(client, baseUrl, headers);
            _models = new ModelService(client, baseUrl, headers);
            _chat = new ChatService(client, baseUrl, headers);
            _templates = new TemplateService(client, baseUrl, headers);
            _conversations = new ConversationService(client, baseUrl, headers);
            _workspaces = new WorkspaceService(client, baseUrl, headers);
            _toolHistory = new ToolHistoryService(client, baseUrl, headers);
            _categories = new CategoryService(client, baseUrl, headers);
            _files = new FileService(client, baseUrl, headers);
            _userSecrets = new UserSecretsService(client, baseUrl, headers);
            _notifications = new NotificationService(client, baseUrl, headers);
        }

        private List<Dictionary<string, object>> BuildMessageList(string userMessage,
            string systemPrompt)
        {
            var messages = new List<Dictionary<string, object>>();
            if (!string.IsNullOrWhiteSpace(systemPrompt))
            {
                messages.Add(new ChatMessage("system", systemPrompt).ToDictionary());
            }
            messages.Add(new ChatMessage("user", userMessage).ToDictionary());
            return messages;
        }

        private static string CheckForEnv(Env envEnum, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) return value;
            var env = envEnum.GetEnvValue();
            if (!string.IsNullOrWhiteSpace(env)) return env;
            throw new ArgumentException(
                $"{envEnum} is required. Provide parameter or set {envEnum} environment variable.");
        }

        private static string CheckForBaseUrl(string baseUrl)
        {
            return CheckForEnv(Env.SAI_API_BASE_URL, baseUrl);
        }

        private static string CheckForApiKey(string apiKey)
        {
            return CheckForEnv(Env.SAI_API_KEY, apiKey);
        }

        public HealthService Health() => _health;
        public ProfileService Profile() => _profile;
        public ModelService Models() => _models;
        public ChatService Chat() => _chat;
        public TemplateService Templates() => _templates;
        public ConversationService Conversations() => _conversations;
        public WorkspaceService Workspaces() => _workspaces;
        public ToolHistoryService ToolHistory() => _toolHistory;
        public CategoryService Categories() => _categories;
        public FileService Files() => _files;
        public UserSecretsService UserSecrets() => _userSecrets;
        public NotificationService Notifications() => _notifications;

        public Result<JsonNode> TestConnection()
        {
            return _health.CheckHealth();
        }

        private void LoadModels()
        {
            var res = _models.GetModels();
            if (!res.IsSuccess)
            {
                Console.WriteLine($"Failed to load models: {res.ErrorMessage}");
                _availableModels = new List<JsonNode>();
                return;
            }

            if (res.Data is JsonArray array)
            {
                _availableModels = array.ToList();
                Console.WriteLine($"Loaded {_availableModels.Count} models");
            }
            else
            {
                _availableModels = new List<JsonNode>();
            }
        }

        public void SetModel(string modelName)
        {
            var found = _availableModels.FirstOrDefault(n =>
                n?["name"]?.GetValue<string>() == modelName);
                
            if (found != null)
            {
                _selectedModel = modelName;
                Console.WriteLine($"Model set to: {modelName}");
                return;
            }

            var names = string.Join(", ", _availableModels
                .Select(n => n?["name"]?.GetValue<string>() ?? "<unknown>"));
                
            throw new InvalidOperationException(
                $"Model '{modelName}' not found. Available: {names}");
        }

        public List<JsonNode> GetChatModels()
        {
            return _models.FilterModelsByType(_availableModels, ModelType.Chat);
        }

        public List<JsonNode> GetAudioModels()
        {
            return _models.FilterModelsByType(_availableModels, ModelType.Audio);
        }

        public List<JsonNode> GetImageModels()
        {
            return _models.FilterModelsByType(_availableModels, ModelType.Image);
        }

        private void ValidateModelIsSelected()
        {
            if (string.IsNullOrWhiteSpace(_selectedModel))
            {
                throw new InvalidOperationException(
                    "No model selected. Use SetModel() to select a model.");
            }
        }

        public Result<JsonNode> SendMessage(string message, string systemPrompt = null,
            Dictionary<string, object> options = null)
        {
            ValidateModelIsSelected();
            
            var messages = BuildMessageList(message, systemPrompt);
            var finalOptions = options ?? new Dictionary<string, object>();
            
            return _chat.SendCompletion(messages, _selectedModel, finalOptions);
        }

        public Result<JsonNode> SendChatWithHistory(
            List<Dictionary<string, object>> messages,
            Dictionary<string, object> options = null)
        {
            if (string.IsNullOrWhiteSpace(_selectedModel))
                return Result<JsonNode>.Error(
                    "No model selected. Use SetModel() to select a model.");
                    
            return _chat.SendCompletion(messages, _selectedModel, options ??
                new Dictionary<string, object>());
        }

        public ChatMessage CreateMessage(string role, string content)
        {
            return new ChatMessage(role, content);
        }

        public ChatMessage CreateMessageWithImage(string role, string text,
            string imageUrl, string detail = "auto")
        {
            return ChatMessage.CreateWithImage(role, text, imageUrl, detail);
        }

        public Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>
            {
                ["SelectedModel"] = _selectedModel,
                ["AvailableModelsCount"] = _availableModels.Count,
                ["ChatModelsCount"] = GetChatModels().Count,
                ["AudioModelsCount"] = GetAudioModels().Count,
                ["ImageModelsCount"] = GetImageModels().Count,
                ["ServicesLoaded"] = new List<string>
                {
                    "Health", "Profile", "Models", "Chat", "Templates",
                    "Conversations", "Workspaces", "ToolHistory", "Categories",
                    "Files", "UserSecrets", "Notifications"
                }
            };
        }

        public void RefreshModels()
        {
            LoadModels();
        }

        public static class Factory
        {
            public static SAILibrary Create(string apiKey)
            {
                return Create(apiKey, Env.SAI_API_BASE_URL.GetEnvValue());
            }

            public static SAILibrary Create(string apiKey, string baseUrl)
            {
                var client = new SystemNetHttpClient();
                return new SAILibrary(apiKey, baseUrl, client);
            }

            public static SAILibrary CreateWithCustomHttpClient(string apiKey,
                string baseUrl, IHttpClientBase client)
            {
                return new SAILibrary(apiKey, baseUrl, client);
            }
        }

        public class ConfigBuilder
        {
            private string _apiKey;
            private string _baseUrl;
            private int _timeoutSeconds = 30;
            private bool _enableRetry = false;
            private int _maxRetries = 3;
            private bool _enableLogging = false;

            public ConfigBuilder WithApiKey(string apiKey)
            {
                _apiKey = apiKey;
                return this;
            }

            public ConfigBuilder WithBaseUrl(string baseUrl)
            {
                _baseUrl = baseUrl;
                return this;
            }

            public ConfigBuilder WithTimeout(int seconds)
            {
                _timeoutSeconds = seconds;
                return this;
            }

            public ConfigBuilder EnableRetryLogic(int maxRetries)
            {
                _enableRetry = true;
                _maxRetries = maxRetries;
                return this;
            }

            public ConfigBuilder EnableRequestLogging()
            {
                _enableLogging = true;
                return this;
            }

            public SAILibrary Build()
            {
                var key = CheckForApiKey(_apiKey);
                IHttpClientBase httpClient = new SystemNetHttpClient
                {
                    TimeoutSeconds = _timeoutSeconds
                };

                if (_enableLogging)
                    httpClient = new LoggingHttpClient(httpClient);
                    
                if (_enableRetry)
                    httpClient = new RetryHttpClient(httpClient, _maxRetries);

                return Factory.CreateWithCustomHttpClient(key, _baseUrl, httpClient);
            }

            private static string CheckForApiKey(string apiKey)
            {
                if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;
                
                var envKey = Env.SAI_API_KEY.GetEnvValue();
                if (!string.IsNullOrWhiteSpace(envKey)) return envKey;
                
                throw new ArgumentException(
                    "API Key is required. Provide parameter or set SAI_API_KEY environment variable.");
            }
        }

        public static class Extensions
        {
            private static Result<T> ExecuteWithRetry<T>(
                Func<Result<T>> operation, int maxRetries)
            {
                int attempt = 0;
                Result<T> lastResult = null;

                while (attempt < maxRetries)
                {
                    attempt++;
                    lastResult = operation();

                    if (lastResult.IsSuccess)
                    {
                        return lastResult;
                    }

                    if (attempt < maxRetries)
                    {
                        WaitBeforeRetry(attempt);
                    }
                }

                return lastResult;
            }

            private static void WaitBeforeRetry(int attempt)
            {
                try
                {
                    var delayMillis = (long)Math.Pow(2, attempt) * 1000L;
                    Thread.Sleep((int)delayMillis);
                }
                catch (ThreadInterruptedException e)
                {
                    throw new Exception("Retry interrupted", e);
                }
            }

            public static Result<JsonNode> ExecuteTemplateWithRetry(SAILibrary sai,
                string templateId, Dictionary<string, object> inputs, int maxRetries)
            {
                return ExecuteWithRetry(() =>
                    sai.Templates().ExecuteTemplate(templateId, inputs), maxRetries);
            }

            public static Result<JsonNode> SendMessageWithRetry(SAILibrary sai,
                string message, string systemPrompt, int maxRetries)
            {
                return ExecuteWithRetry(() =>
                    sai.SendMessage(message, systemPrompt), maxRetries);
            }

            public static List<string> ExtractTextFromChatResponse(
                Result<JsonNode> chatResult)
            {
                if (chatResult == null || !chatResult.IsSuccess || chatResult.Data == null)
                    return new List<string>();

                var list = new List<string>();
                var root = chatResult.Data;

                if (root["choices"] is JsonArray choices)
                {
                    foreach (var choice in choices)
                    {
                        if (choice?["message"]?["content"] != null)
                        {
                            list.Add(choice["message"]["content"].ToString());
                        }
                    }
                }

                return list;
            }

            public static Dictionary<string, object> CreateConversationContext(
                List<Dictionary<string, object>> messages)
            {
                var context = new Dictionary<string, object>
                {
                    ["MessageCount"] = messages?.Count ?? 0
                };

                var roles = new List<string>();
                int totalLength = 0;

                if (messages != null)
                {
                    foreach (var m in messages)
                    {
                        if (m.TryGetValue("role", out var role) && role is string roleStr)
                            roles.Add(roleStr);

                        if (m.TryGetValue("content", out var content))
                        {
                            if (content is string strContent)
                                totalLength += strContent.Length;
                        }
                    }
                }

                context["Roles"] = roles;
                context["TotalLength"] = totalLength;
                context["UniqueRoles"] = roles.Distinct().ToList();

                return context;
            }
        }
    }
}