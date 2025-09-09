using APICatalago.Models;
using ApiCatalogo.Helpers;
using ApiCatalogo.Services.AiServices;
using ApiCatalogo.Services.AiServices.AiServices.DTOs;
using ApiCatalogo.Services.AiServices.Enums;
using ApiCatalogo.Services.AiServices.Helpers;
using ApiCatalogo.Services.AiServices.Interfaces;
using ApiCatalogo.Services.AiServices.ModelsPrompt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatAIController : ControllerBase
    {
        private readonly string _apiKey = "AIzaSyDikJ4VfagbGchH97lJPGUucnOBO0yeGpo";
        private readonly IAiService __aiService;
        private readonly SqlQueryExecutor _sqlQueryExecutor;
        public ChatAIController(IAiService aiService, SqlQueryExecutor sqlQueryExecutor)
        {
            __aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _sqlQueryExecutor = sqlQueryExecutor ?? throw new ArgumentNullException(nameof(sqlQueryExecutor));
        }


        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] QuestionRequestDTO questionRequest)
        {
            if (string.IsNullOrWhiteSpace(questionRequest.MessageUser))
                return BadRequest("Question é obrigatória.");

            if (questionRequest.ModePrompt == ModePrompt.Sql)
            {
                __aiService.SetPromptGenerator(new SqlPrompt());
                var result = await __aiService.Send(questionRequest);
                if (result is not OkObjectResult okResult)
                    return BadRequest("Não foi possível processar a solicitação.");

                string modelReply = okResult.Value as string ?? string.Empty;
                string sqlQuery = AiSqlHelper.CleanSqlFromModel(modelReply);

                if (!AiSqlHelper.IsSafeSelectQuery(sqlQuery))
                    return BadRequest("Query não permitida. Só aceitamos SELECTs de leitura simples.");

                var dados = await _sqlQueryExecutor.ExecuteQueryAsync(sqlQuery);
                return Ok(dados);
            }
            else if (questionRequest.ModePrompt == ModePrompt.HtmlDashBoard)
            {
                __aiService.SetPromptGenerator(new HtmlDashboardPrompt());
                var htmlResult = await __aiService.Send(questionRequest);

                if (htmlResult is not OkObjectResult htmlOkResult)
                    return BadRequest("Não foi possível processar a solicitação.");

                string html = htmlOkResult.Value as string ?? string.Empty;
                return Content(html, "text/html");
            }
            else
            {
                return BadRequest("ModePrompt inválido.");
            }
        }

    }
}

