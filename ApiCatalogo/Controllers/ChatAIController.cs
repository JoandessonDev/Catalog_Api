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

            if (!IsSafeSelectQuery(sqlQuery))
                return BadRequest("Query não permitida. Só aceitamos SELECTs de leitura simples.");

            var dados = await _sqlQueryExecutor.ExecuteQueryAsync(sqlQuery);

            return Ok(dados);
        }

        private bool IsSafeSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            string noComments = Regex.Replace(sql, @"(--.*?$)|(/\*.*?\*/)", "", RegexOptions.Singleline | RegexOptions.Multiline);
            noComments = noComments.Trim();
            if (string.IsNullOrEmpty(noComments))
                return false;

            var parts = noComments.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();
            if (parts.Length != 1)
                return false;

            string statement = parts[0];

            if (!Regex.IsMatch(statement, @"^\s*(SELECT|WITH)\b", RegexOptions.IgnoreCase))
                return false;

            var forbiddenPattern = @"\b(INSERT|UPDATE|DELETE|DROP|TRUNCATE|ALTER|CREATE|GRANT|REVOKE|MERGE|EXEC|EXECUTE|CALL|ATTACH|DETACH|INTO|REPLACE|UPSERT)\b";
            if (Regex.IsMatch(noComments, forbiddenPattern, RegexOptions.IgnoreCase))
                return false;

            return true;
        }


    }
}

