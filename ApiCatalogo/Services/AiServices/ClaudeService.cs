using ApiCatalogo.Services.AiServices.AiServices.DTOs;
using ApiCatalogo.Services.AiServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

public class ClaudeService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly List<object> _historyChat = new();
    private IPromptGenerator _promptGenerator;

    public ClaudeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["ClaudeApiKey"] ?? throw new ArgumentNullException("ClaudeApiKey");
    }

    public async Task<IActionResult> Send(QuestionRequestDTO questionRequest)
    {
        if (string.IsNullOrWhiteSpace(questionRequest.MessageUser))
            return new BadRequestObjectResult("question não pode ser vazio");

        var prompt = _promptGenerator.GeneratePrompt(questionRequest);

        _historyChat.Add(new { role = "user", content = prompt });

        var payload = new
        {
            model = "claude-2",
            messages = _historyChat,
            max_tokens_to_sample = 1000
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey); 

        try
        {
            var response = await _httpClient.PostAsync("https://api.anthropic.com/v1/complete", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new ObjectResult(new { error = result }) { StatusCode = (int)response.StatusCode };

            using var doc = JsonDocument.Parse(result);
            var modelReply = doc.RootElement
                .GetProperty("completion")
                .GetString();

            _historyChat.Add(new { role = "assistant", content = modelReply });
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
