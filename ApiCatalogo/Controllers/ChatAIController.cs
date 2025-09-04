using APICatalago.Models;
using ApiCatalogo.Helpers;
using ApiCatalogo.Services;
using ApiCatalogo.Services.AiServices;
using ApiCatalogo.Services.Interfaces;
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
        public async Task<IActionResult> Send([FromBody] string questionRequest)
        {
            if (string.IsNullOrWhiteSpace(questionRequest))
                return BadRequest("Question é obrigatória.");

            var result = await __aiService.Send(questionRequest);
            if (result is not OkObjectResult okResult)
                return BadRequest("Não foi possível processar a solicitação.");

            string modelReply = okResult.Value as string ?? string.Empty;
            string sqlQuery = AiSqlHelper.CleanSqlFromModel(modelReply);

            // Segurança: permitir apenas SELECT
            var forbiddenCommands = new[] { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "TRUNCATE", "EXEC" };
            var upperSql = sqlQuery.ToUpperInvariant();

            if (!upperSql.TrimStart().StartsWith("SELECT"))
                return BadRequest("A query precisa ser um SELECT.");

            if (forbiddenCommands.Any(cmd => upperSql.Contains(cmd)))
                return BadRequest("A query contém operações não permitidas. Apenas SELECT é permitido.");

            // Executa a query de forma dinâmica
            var dados = await _sqlQueryExecutor.ExecuteQueryAsync(sqlQuery);

            return Ok(dados);
        }


    }
}

