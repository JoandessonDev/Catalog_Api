using ApiCatalogo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ApiCatalogo.Models.DTOs;
using ApiCatalogo.Repositories.Interfaces;
using System.Text.Json.Nodes;

namespace ApiCatalogo.Services.AiServices
{
    public class GeminiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly List<object> _historyChat = new();
        public ISchemaRepository SchemaRepository { get; }


        public GeminiService(IHttpClientFactory httpClientFactory, ISchemaRepository schemaRepository, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["GeminiApiKey"] ?? throw new ArgumentNullException("GeminiApiKey");
            SchemaRepository = schemaRepository ?? throw new ArgumentNullException(nameof(schemaRepository));
        }


        public async Task<IActionResult> Send(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return new BadRequestObjectResult("question não pode ser vazio");


            var prompt = BuildPromptForQueryGeneration(question);

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


        public string BuildPromptForQueryGeneration(string question)
        {
            var prompt = $@"
                Você é um assistente que gera consultas SQL seguras para meu sistema .NET/EF Core.
                Não retorne explicações — retorne apenas a query SQL pura, no formato válido para SQLite/MySQL (compatível com EF Core).
                Não esqueça de usar os padrões utilizados pelo Entity Framework, como o uso de nomes de tabelas no plural (ex: Produtos, Categorias).
                Use os modelos a baixo que são utilizados a baixo pelo Entity Framework para acessar a base de dados:
                DataBase=CatalogDB
                Cliente: Id, Nome, Email, Telefone, Endereco, DataCadastro
                Venda: Id, ClienteId, DataVenda, Total, Status
                VendaItem: Id, VendaId, ProdutoId, PrecoUnitario, Quantidade, SubTotal 
                Produto: Id, Nome, Descricao, Preco, ImageUrl, Estoque, DataCadastro, CategoriaId
                Categoria: Id, Nome, ImageUrl

                Regras obrigatórias:           

                Nunca use SELECT *.

                Se houver JOIN, use aliases para que os nomes das colunas no SELECT correspondam exatamente aos nomes do modelo.

                Sempre retorne apenas uma instrução SELECT.

                Coloque parâmetros de busca na query conforme solicitado (@param).

                Não inclua comentários, JSON, aspas extras, caracteres escapados ou múltiplas instruções.

                Priorize keywords em maiúsculas (SELECT, FROM, JOIN, WHERE, ORDER BY, LIMIT).

                Se o que o usuário pedir não for possível com o esquema, retorne apenas SELECT 1;.

                Importante:

                Mesmo que a pergunta seja sobre agregação, ranking ou filtros, não omita nenhuma coluna do modelo no SELECT a não ser que eu peça para que traga
                dados especificos como só trazer o nome ou outras colunas de foma especifica.

                Use aliases se necessário para evitar conflitos de nomes.

                Se a tabela tem CategoriaId ou qualquer coluna obrigatória, ela deve aparecer no SELECT, mesmo que não seja usada na lógica da pergunta.
             
                Pergunta do usuário: ""{question}""


                Regras:
                - sempre que alguma consulta retornar numeros com casas decimais, ponha as casas em 2 caracteres a não ser que eu tenha pedido por uma quantidade diferente.
                - Retorne apenas a query SQL, sem JSON, sem explicação, sem comentários.
                - Use apenas tabelas e colunas do esquema informado.
                - Inclua SELECT com colunas específicas, nunca use SELECT *.
                - Inclua JOIN quando for necessário acessar dados de outra tabela (ex: Categoria).
                Regras obrigatórias (siga exatamente):
                1. Retorne **somente** a instrução SQL. **Sem** fences ``` ou ```sql, **sem** texto adicional.
                2. Use sempre os nomes de colunas exatamente como no esquema (ex.: CategoriaId).
                3. Sempre inclua no SELECT o nome de todas as colunas dos modelos sem deixar passar senhum para que não de erro por falta de coluna
                4. Nunca use `SELECT *`.
                
                6. Coloque os parametros de busca na query conforme a solicitação do usuário
                7. Não inclua comentários SQL ou instruções múltiplas (apenas 1 SELECT por resposta).
                8. Não inclua caracteres escapados (`\r\n`) nem aspas extra. Quebras de linha são permitidas como linhas normais.
                9. Priorize clareza: keywords em MAIÚSCULAS é preferível (SELECT, FROM, WHERE, JOIN, ORDER BY, LIMIT).
                10. Se a pergunta pedir algo que não é possível com o esquema, retorne apenas: `SELECT 1;`
                Sempre inclua todas as colunas do modelo no SELECT, exatamente como no esquema (ex: CategoriaId), para evitar erros do EF Core.

                Nunca use SELECT *.

                Se houver JOIN, use aliases para que os nomes das colunas no SELECT correspondam exatamente aos nomes do modelo.

                Sempre retorne apenas uma instrução SELECT.

                Coloque os parâmetros de busca na query conforme a solicitação do usuário (@param).

                Não inclua comentários, JSON, aspas extras, caracteres escapados ou múltiplas instruções.

                Priorize clareza e keywords em maiúsculas (SELECT, FROM, JOIN, WHERE, ORDER BY).
                IMPORTANTE: SEMPRE USAR PASCAL CASE O EF vai colocar as colunas com pascalcase e no plural não esqueça disso ex: vendaItems.
            ";
            return prompt;
        }



        //public async Task<IActionResult> Send(string question)
        //{
        //    if (string.IsNullOrWhiteSpace(question))
        //        return new BadRequestObjectResult("question não pode ser vazio");

        //    var askToAiRequestDTO = new AskToAiRequestDTO
        //    {
        //        Schema = await SchemaRepository.GetSchemaAsync(),
        //        Question = question
        //    };

        //    var prompt = BuildPromptForQueryGeneration(askToAiRequestDTO.Schema, askToAiRequestDTO.Question);

        //    _historyChat.Add(new
        //    {
        //        role = "user",
        //        parts = new[] { new { text = prompt } }
        //    });

        //    var payload = new { contents = _historyChat };

        //    var json = JsonSerializer.Serialize(payload);
        //    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        //    using var request = new HttpRequestMessage(HttpMethod.Post,
        //        "v1beta/models/gemini-2.5-flash:generateContent")
        //    {
        //        Content = httpContent
        //    };
        //    request.Headers.Clear();
        //    request.Headers.Add("x-goog-api-key", _apiKey);

        //    try
        //    {
        //        var response = await _httpClient.SendAsync(request);
        //        var resultString = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return new ObjectResult(new { error = resultString }) { StatusCode = (int)response.StatusCode };
        //        }

        //        using var doc = JsonDocument.Parse(resultString);
        //        string modelReply = null;
        //        if (doc.RootElement.TryGetProperty("candidates", out var candidates)
        //            && candidates.GetArrayLength() > 0
        //            && candidates[0].TryGetProperty("content", out var contentEl)
        //            && contentEl.TryGetProperty("parts", out var parts)
        //            && parts.GetArrayLength() > 0
        //            && parts[0].TryGetProperty("text", out var textEl))
        //        {
        //            modelReply = textEl.GetString();
        //        }
        //        else
        //        {
        //            modelReply = "Resposta em formato inesperado.";
        //        }

        //        _historyChat.Add(new
        //        {
        //            role = "model",
        //            parts = new[] { new { text = modelReply } }
        //        });

        //        return new OkObjectResult(new { reply = modelReply });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        //    }
        //}
    }
}
