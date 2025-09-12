using ApiCatalogo.Services.AiServices.AiServices.DTOs;
using ApiCatalogo.Services.AiServices.Helpers;
using ApiCatalogo.Services.AiServices.Interfaces;

namespace ApiCatalogo.Services.AiServices.ModelsPrompt
{
    public class HtmlPrompt : IPromptGenerator
    {
        public string GeneratePrompt(QuestionRequestDTO questionRequest)
        {
            var data = PrompJsonListObject.PrepareListJsonForPrompt(questionRequest.ListObject);
            var prompt = $@"
                Use exatamente o conteúdo abaixo como os dados PARA EXIBIR O QUE FOR SOLICIDATO:
                    {data}  
                 
                IMPORTANTE SE VC RECEBEU UM DADO COMO SE EU TIVESSE DADO UM SELECT 1 NA MINHA BASE EXIBA UMA MENSAGEM DE ERRO. SE O DADO QUE CHEGOU A VC FOI UM 1. Só que não diga nada a respeito disso apenas diga que não é possivel atender a esta solicitação NUNCA DIGA  coisas do tipo Os dados fornecidos são equivalentes a um SELECT 1 em uma base de dados, o que não é um formato válido para processamento ou exibição.. 
                iSE VC NÃO RECEBER NENHUM DADO ANALISE UNICAMENTE A MENSAGEM DO USUÁRIO OU MOSTRE UM HTML DIZENDO QUE NÃO FOI POSSIVEL ATENDER A ESTA SOLICITAÇÃO. OU GERE ALGUMA MENSAGEM DE ERRO TBM
                não coloque paragrafos ou textos proximos a tabela na tabela no maximo um unico titulo em cima da tabela nada mais.
                GERE APENAS UM ARQUIVO HTML COMPLETO (inicie com <!doctype html> e termine com </html>). 
                como base nos dados fornecidos me mostreem forma de TABELA HTML os dados fornecidos.
                sÓ GERE UM DASHBOARD SE EU PEDIR EXPRESSAMENTE NA PERGUNTA. CASO CONTRÁRIO, APENAS UMA TABELA HTML.
                CASO EU PEÇA UM DASHBOAR UTILIZE AS INSTRUÇÕES ABAIXO:
                NADA MAIS: não escreva explicações, não inclua texto fora do HTML, sem markdown, sem comentários de instrutor. O HTML deve conter um dashboard responsivo que use Chart.js (pode importar via CDN).
                CASO EU QUEIRA ESPECIFICAR O TIPO DE GRÁFICO, EU VOU DIZER NA PERGUNTA. CASO CONTRÁRIO, ESCOLHA OS TIPOS DE GRÁFICOS MAIS ADEQUADOS PARA OS DADOS FORNECIDOS.
                POSSO ESTAR PASSANDO DADOS PARA TABELAS DESEJADAS OU CORES A SEREM UTILIZADAS . USE-OS CONFORME A PERGUNTA.

                Pergunta do usuário: ""{questionRequest.MessageUser}""

                DADOS DE ENTRADA:
               

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
                12- se for gerar uma tabela apenas use essa formatação aqui
                body{{font-family:Arial,sans-serif;margin:20px;background-color:#f4f4f4;color:#333}}h1{{color:#333;text-align:center;margin-bottom:30px}}
                table{{width:90%;margin:0 auto;border-collapse:collapse;box-shadow:0 2px 10px rgba(0,0,0,0.1);background-color:#fff;caption-side:top}}th,td{{padding:12px 15px;text-align:left;border-bottom:1px solid #ddd}}
                th{{background-color:#5F59B5;color:#fff;font-weight:bold}}tr:nth-child(even){{background-color:#f9f9f9}}tr:hover{{background-color:#f1f1f1}}@media(max-width:768px){{table{{width:100%}}th,td{{padding:8px 10px}}}}
                
                13 - Use as cores acima pra construir titulos e headers porem quando for gerar graficos use cores diferentes coloridas pra representar melhor a diferença entre os dados.
                14- ao montar um dashboard nunca coloque uma tabela em msm linha que graficos sempre um em baixo do outro
                Lembre-se: o arquivo de saída deve ser **APENAS** o documento HTML completo, pronto para salvar como .html e abrir no navegador. Nada fora do HTML.
            ";
            return prompt;
        }
    }
}
