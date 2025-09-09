using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ApiCatalogo.Repositories.Interfaces;
using System.Text.Json.Nodes;
using ApiCatalogo.Services.AiServices.Interfaces;
using ApiCatalogo.Services.AiServices.AiServices.DTOs;

namespace ApiCatalogo.Services.AiServices
{
    public class GeminiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly List<object> _historyChat = new();
        private IPromptGenerator _promptGenerator;
    


        public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["GeminiApiKey"] ?? throw new ArgumentNullException("GeminiApiKey");
        }


        public async Task<IActionResult> Send(QuestionRequestDTO questionRequest)
        {
            if (string.IsNullOrWhiteSpace(questionRequest.MessageUser))
                return new BadRequestObjectResult("question não pode ser vazio");


            var prompt = _promptGenerator.GeneratePrompt(questionRequest);

            _historyChat.Add(new
            {
                role = "user",
                parts = new[] { new { text = prompt } }
            });

            var payload = new { contents = _historyChat };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(
                    "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent",
                    content
                );
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ObjectResult(new { error = result }) { StatusCode = (int)response.StatusCode };
                }

                using var doc = JsonDocument.Parse(result);
                var modelReply = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
               

                _historyChat.Add(new
                {
                    role = "model",
                    parts = new[] { new { text = modelReply } }
                });
                _historyChat.Clear();
                return new OkObjectResult(modelReply);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }

        public void SetPromptGenerator(IPromptGenerator promptGenerator)
        {
            _promptGenerator = promptGenerator ?? throw new ArgumentNullException(nameof(promptGenerator) + " não pode ser nulo");
        }
    }
}
