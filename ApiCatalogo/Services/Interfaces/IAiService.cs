using ApiCatalogo.Models;
using ApiCatalogo.Models.DTOs;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Services.Interfaces
{
    public interface IAiService
    {
        Task<IActionResult> Send(string question);
        string BuildPromptForQueryGeneration(string question);
    }
}
