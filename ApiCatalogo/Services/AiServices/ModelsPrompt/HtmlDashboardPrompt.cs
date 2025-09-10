using ApiCatalogo.Services.AiServices.AiServices.DTOs;
using ApiCatalogo.Services.AiServices.Helpers;
using ApiCatalogo.Services.AiServices.Interfaces;

namespace ApiCatalogo.Services.AiServices.ModelsPrompt
{
    public class HtmlDashboardPrompt : IPromptGenerator
    {
        public string GeneratePrompt(QuestionRequestDTO questionRequest)
        {
            var data = PrompJsonListObject.PrepareListJsonForPrompt(questionRequest.ListObject);
            var prompt = $@"
                GERE APENAS UM ARQUIVO HTML COMPLETO (inicie com <!doctype html> e termine com </html>). NADA MAIS: não escreva explicações, não inclua texto fora do HTML, sem markdown, sem comentários de instrutor. O HTML deve conter um dashboard responsivo que use Chart.js (pode importar via CDN). 
                
                CASO EU QUEIRA ESPECIFICAR O TIPO DE GRÁFICO, EU VOU DIZER NA PERGUNTA. CASO CONTRÁRIO, ESCOLHA OS TIPOS DE GRÁFICOS MAIS ADEQUADOS PARA OS DADOS FORNECIDOS.
                POSSO ESTAR PASSANDO DADOS PARA TABELAS DESEJADAS OU CORES A SEREM UTILIZADAS . USE-OS CONFORME A PERGUNTA.

                Pergunta do usuário: ""{questionRequest.MessageUser}""

                DADOS DE ENTRADA:
                Use exatamente o conteúdo abaixo como os dados a serem exibidos no dashboard:
                IMPORTANTE! DADOS PARA GERAR O DASHBOARD: 
                    {data}

                REGRAS OBRIGATÓRIAS:
                1. Saída: APENAS o HTML completo. Nenhuma linha fora do HTML.
                2. Dependência externa permitida: apenas Chart.js via CDN (ex.: https://cdn.jsdelivr.net/npm/chart.js). Tudo o mais deve ser inline (CSS e JS no próprio HTML).
                3. Estrutura: forneça uma seção com CSS responsivo e um layout com múltiplos painéis (cards) para gráficos, tabelas e sumários.
                4. deifina bosn limites para qeu os graficos não se exapandam de forma indeifinida e gere o erro qeu faz o chart crescer na tela inteira infinitamente
                5. Segurança contra erros:
                   - Antes de inicializar cada chart, valide se os arrays de labels e datasets existem e têm o mesmo comprimento. Se faltar dado, NÃO inicialize o chart e mostre um placeholder visual (texto no card: \""Sem dados disponíveis\"").
                   - Envolver a inicialização em `try {{ ... }} catch (e) {{ mostraMensagemDeErroVisivelNoCard(e); }}` para evitar falhas que quebrem o restante do dashboard.
                6. Melhor experiência:
                   - Defina opções Chart.js com `animation: {{ duration: 300 }}`, `plugins.tooltip.mode = 'index'`, `plugins.legend.position = 'top'`.
                   - Para eixos com muitos labels, habilitar `ticks.autoSkip = true` e `ticks.maxRotation = 0`.
                   - Use `scales.y.beginAtZero = true` quando for gráfico de quantidade/valor.
                7. Acessibilidade & estilo:
                   - Use tags semânticas (header, main, section, nada de footer).
                   - Cada gráfico deve ter `aria-label` e `<h3>` com título.
                8. Dados dinâmicos embutidos:
                   - Insira os dados a serem usados em um elemento `<script id=\""dashboard-data\"" type=\""application/json\""> ... </script>` contendo o JSON (use o conteúdo de entrada acima convertido para JSON quando for apropriado).
                   - O JS do HTML deve ler esse JSON e criar os gráficos dinamicamente.
                9. Fallbacks:
                   - Se Chart.js falhar por algum motivo, exibir um resumo textual dos valores principais (total, média, máximo, mínimo) dentro do card correspondente.
                10. Compactação e limpeza:
                   - Código claro, indentado, sem comentários explicativos externos; somente comentários mínimos inline se necessário para robustez.
                11. Não incluir:
                   - Não inclua chamadas a APIs externas além do CDN do Chart.js.
                   - Não inclua bibliotecas CSS/JS extras (Bootstrap, jQuery etc.) a menos que estritamente necessário — PRIORIZE HTML/CSS/JS puro + Chart.js.

                Lembre-se: o arquivo de saída deve ser **APENAS** o documento HTML completo, pronto para salvar como .html e abrir no navegador. Nada fora do HTML.
                
            ";
            return prompt;
        }
    }
}
