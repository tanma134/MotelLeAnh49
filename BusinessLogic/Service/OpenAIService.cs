using System.Net.Http.Json;
using System.Text.Json;
using BusinessLogic.Config;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Service
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _http;
        private readonly OpenAIConfig _config;

        public OpenAIService(HttpClient http, IOptions<OpenAIConfig> config)
        {
            _http = http;
            _config = config.Value;
        }

        public async Task<string> SendPromptAsync(
            string userMessage,
            IEnumerable<(string role, string content)>? history = null)
        {
            try
            {
                var prompt = _config.SystemPrompt + "\n\nUser: " + userMessage;

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var url =
                    $"https://generativelanguage.googleapis.com/v1beta/models/{_config.Model}:generateContent?key={_config.ApiKey}";

                Console.WriteLine("===== GEMINI REQUEST =====");
                Console.WriteLine(JsonSerializer.Serialize(payload));

                var response = await _http.PostAsJsonAsync(url, payload);

                var raw = await response.Content.ReadAsStringAsync();

                Console.WriteLine("===== GEMINI RESPONSE =====");
                Console.WriteLine(raw);

                response.EnsureSuccessStatusCode();

                using var doc = JsonDocument.Parse(raw);

                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "No response received.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("===== AI ERROR =====");
                Console.WriteLine(ex.Message);

                return "Xin lỗi, AI đang gặp sự cố.";
            }
        }
    }
}