using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Sai_Library.Http;
using Sai_Library.Models;
using UriBuilder = Sai_Library.Utils.UriBuilder;

namespace Sai_Library.Services
{
    public class FileService : BaseService
    {
        public FileService(IHttpClientBase httpClient, string baseUrl, 
            Dictionary<string, string>? headers) : base(httpClient, baseUrl, headers)
        {
        }

        public Result<JsonNode> GetUploadToken(string containerName = null, 
            string filename = null, string folder = null)
        {
            var query = new Dictionary<string, object>
            {
                ["folder"] = folder ?? "useruploads"
            };

            if (!string.IsNullOrEmpty(containerName))
                query["containerName"] = containerName;
                
            if (!string.IsNullOrEmpty(filename))
                query["filename"] = filename;

            return Get("/api/storage/uploadtoken", query);
        }

        public async Task<Result<JsonNode>> UploadFileAsync(string filePath, 
            string model = null)
        {
            if (!File.Exists(filePath))
                return Result<JsonNode>.Error($"File not found: {filePath}");

            var query = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(model))
                query["model"] = model;

            var uri = Utils.UriBuilder.Build(BaseUrl, "/api/provider-files/upload", query);

            try
            {
                using var client = new HttpClient();
                using var form = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);
                using var fileContent = new StreamContent(fileStream);
        
                fileContent.Headers.ContentType = 
                    MediaTypeHeaderValue.Parse("application/octet-stream");
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                if (Headers != null)
                {
                    foreach (var header in Headers.Where(h => 
                        h.Key != "Content-Type"))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(
                            header.Key, header.Value);
                    }
                }

                var response = await client.PostAsync(uri, form);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var json = JsonNode.Parse(responseBody);
                    return Result<JsonNode>.Success(json, new Dictionary<string, object>
                    {
                        ["status"] = (int)response.StatusCode
                    });
                }

                return Result<JsonNode>.Error(
                    $"HTTP {(int)response.StatusCode}: {responseBody}");
            }
            catch (Exception ex)
            {
                return Result<JsonNode>.Error($"File upload failed: {ex.Message}");
            }
        }

        public Result<JsonNode> DownloadFile(string model = null, string fileId = null)
        {
            var query = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(model))
                query["model"] = model;
                
            if (!string.IsNullOrEmpty(fileId))
                query["fileId"] = fileId;

            return Get("/api/provider-files/download", query);
        }
    }
}