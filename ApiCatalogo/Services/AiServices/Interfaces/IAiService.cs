using ApiCatalogo.Models;
using ApiCatalogo.Repositories.Interfaces;
using ApiCatalogo.Services.AiServices.AiServices.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Services.AiServices.Interfaces
{
    public interface IAiService
    {
        void SetPromptGenerator(IPromptGenerator promptGenerator);
        Task<IActionResult> Send(QuestionRequestDTO questionRequest);
    }
}
