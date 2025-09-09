using ApiCatalogo.Services.AiServices.AiServices.DTOs;

namespace ApiCatalogo.Services.AiServices.Interfaces
{
    public interface IPromptGenerator
    {
        string GeneratePrompt(QuestionRequestDTO questionRequest);
    }
}
