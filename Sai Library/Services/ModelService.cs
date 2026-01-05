using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Sai_Library.Enums;
using Sai_Library.Http;
using Sai_Library.Models;

namespace Sai_Library.Services
{
    public class ModelService : BaseService
    {
        public ModelService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetModels()
        {
            return Get("/api/models");
        }

        public Result<JsonNode> GetRealtimeModels()
        {
            return Get("/api/models/realtime");
        }

        public List<JsonNode> FilterModelsByType(JsonNode? models, ModelType type)
        {
            // Verifica se models é um JsonArray
            if (models == null || !(models is JsonArray))
            {
                return new List<JsonNode>();
            }
            var jsonArray = models.AsArray();
            var result = new List<JsonNode>();
            foreach (var node in jsonArray)
            {
                // Verifica se o nó tem a propriedade "type" e se corresponde ao tipo desejado
                if (node?["type"]?.GetValue<int>() == (int)type)
                {
                    result.Add(node);
                }
            }
            return result;
        }

        public List<JsonNode> FilterModelsByType(List<JsonNode>? models, ModelType type)
        {
            if (models == null) return new List<JsonNode>();
            
            return models.Where(node => 
                node?["type"]?.GetValue<int>() == (int)type).ToList();
        }
    }
}