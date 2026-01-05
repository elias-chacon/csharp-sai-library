using System.Collections.Generic;
using System.Text.Json.Nodes;
using Sai_Library.Enums;
using Sai_Library.Models;

namespace Sai_Library.Http
{
    public interface IHttpClientBase
    {
        int TimeoutSeconds { get; set; }

        Result<JsonNode> MakeRequest(string uri, RequestMethod method,
            Dictionary<string, string>? headers = null, string? body = null);
    }
}