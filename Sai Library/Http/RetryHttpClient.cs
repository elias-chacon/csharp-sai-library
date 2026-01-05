using System.Threading;
using System.Text.Json.Nodes;
using Sai_Library.Enums;
using Sai_Library.Models;

namespace Sai_Library.Http
{
    public class RetryHttpClient : IHttpClientBase
    {
        private readonly IHttpClientBase _innerClient;
        private readonly int _maxRetries;
        
        public int TimeoutSeconds
        {
            get => _innerClient.TimeoutSeconds;
            set => _innerClient.TimeoutSeconds = value;
        }

        public RetryHttpClient(IHttpClientBase innerClient, int maxRetries)
        {
            _innerClient = innerClient ?? throw new ArgumentNullException(nameof(innerClient));
            _maxRetries = Math.Max(1, maxRetries);
        }

        public Result<JsonNode> MakeRequest(string uri, RequestMethod method, 
            Dictionary<string, string> headers = null, string body = null)
        {
            int attempt = 0;
            Result<JsonNode> lastResult = null;

            do
            {
                attempt++;
                lastResult = _innerClient.MakeRequest(uri, method, headers, body);

                if (lastResult.IsSuccess)
                {
                    return lastResult;
                }

                if (attempt < _maxRetries)
                {
                    var delaySeconds = (long)Math.Pow(2, attempt);
                    Console.WriteLine($"🔄 Retry {attempt}/{_maxRetries} in {delaySeconds}s ({lastResult.ErrorMessage})");
                    Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
            } while (attempt < _maxRetries);

            return lastResult;
        }
    }
}