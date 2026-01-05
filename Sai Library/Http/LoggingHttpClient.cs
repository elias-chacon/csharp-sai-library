using System.Diagnostics;
using System.Text.Json.Nodes;
using Sai_Library.Enums;
using Sai_Library.Models;

namespace Sai_Library.Http
{
    public class LoggingHttpClient : IHttpClientBase
    {
        private readonly IHttpClientBase _innerClient;

        public int TimeoutSeconds
        {
            get => _innerClient.TimeoutSeconds;
            set => _innerClient.TimeoutSeconds = value;
        }

        public LoggingHttpClient(IHttpClientBase innerClient)
        {
            _innerClient = innerClient ?? throw new ArgumentNullException(nameof(innerClient));
        }

        public Result<JsonNode> MakeRequest(string uri, RequestMethod method,
            Dictionary<string, string> headers = null, string body = null)
        {
            Console.WriteLine($"🌐 {method} {uri}");
            var stopwatch = Stopwatch.StartNew();

            var result = _innerClient.MakeRequest(uri, method, headers, body);

            stopwatch.Stop();
            var status = result.IsSuccess ? "✅" : "❌";
            Console.WriteLine($"{status} Completed in {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }
    }
}