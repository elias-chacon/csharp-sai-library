using System.Collections;
using System.Text;
using System.Web;

namespace Sai_Library.Utils
{
    public static class UriBuilder
    {
        public static string Build(string baseUri, string endpoint, 
            Dictionary<string, object> queryParams)
        {
            var builder = new StringBuilder();
            builder.Append(baseUri.TrimEnd('/'));
            
            if (!endpoint.StartsWith("/"))
                builder.Append('/');
                
            builder.Append(endpoint);

            if (queryParams != null && queryParams.Any())
            {
                builder.Append('?');
                builder.Append(BuildQueryString(queryParams));
            }

            return builder.ToString();
        }

        private static string BuildQueryString(Dictionary<string, object> queryParams)
        {
            var pairs = new List<string>();
            
            foreach (var param in queryParams)
            {
                if (param.Value == null) continue;
                
                if (param.Value is IEnumerable enumerable && 
                    !(param.Value is string))
                {
                    foreach (var item in enumerable)
                    {
                        pairs.Add($"{HttpUtility.UrlEncode(param.Key)}={HttpUtility.UrlEncode(item?.ToString() ?? "")}");
                    }
                }
                else
                {
                    pairs.Add($"{HttpUtility.UrlEncode(param.Key)}={HttpUtility.UrlEncode(param.Value?.ToString() ?? "")}");
                }
            }
            
            return string.Join("&", pairs);
        }
    }
}