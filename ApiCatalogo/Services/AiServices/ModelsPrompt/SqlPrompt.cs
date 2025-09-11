using ApiCatalogo.Services.AiServices.AiServices.DTOs;
using ApiCatalogo.Services.AiServices.Interfaces;

namespace ApiCatalogo.Services.AiServices.ModelsPrompt
{
    public class SqlPrompt : IPromptGenerator
    {
        public string GeneratePrompt(QuestionRequestDTO questionResponse)
        {
            var prompt = $@"
                FOQUE APENAS NAS INSTRUÇÕES SOBRE A CONSULTA QUALQUER OUTRA DESCRIÇÃO DE GERAÇÃO DE TABELAS GRAFICOS OU QUAQUER COISA QUE SEJA IGNORE, FOQUE APENAS EM GERAR UNICA E EXCLUSIVAMENTE A QUERY.
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
             
                Pergunta do usuário: ""{questionResponse.MessageUser}""


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
    }
}
