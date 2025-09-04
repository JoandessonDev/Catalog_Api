using System.Text.RegularExpressions;

namespace ApiCatalogo.Helpers
{
    public class AiSqlHelper
    {
        public static string CleanSqlFromModel(string modelReply)
        {
            if (string.IsNullOrWhiteSpace(modelReply))
                return modelReply ?? string.Empty;

            string s = modelReply.Trim();

            // Se estiver inteiramente entre aspas "..." ou '...'
            if ((s.Length >= 2) && ((s[0] == '"' && s[^1] == '"') || (s[0] == '\'' && s[^1] == '\'')))
                s = s.Substring(1, s.Length - 2).Trim();

            // Remove fences de código tipo ```sql\n ... ``` (início e fim)
            // e fences simples ``` ... ```
            s = Regex.Replace(s, @"^```[a-zA-Z]*\r?\n", string.Empty, RegexOptions.IgnoreCase); // remove ```sql\n ou ```\n no começo
            s = Regex.Replace(s, @"\r?\n```$", string.Empty, RegexOptions.IgnoreCase); // remove ``` no fim
            s = Regex.Replace(s, @"^```$", string.Empty, RegexOptions.IgnoreCase); // caso só seja fence

            // Remove prefixo "sql" ou "SQL" no começo seguido de nova linha
            s = Regex.Replace(s, @"^\s*(sql|SQL)\s*[:\-]*\s*\r?\n", string.Empty, RegexOptions.IgnoreCase);

            // Caso o modelo tenha usado backticks simples `...`
            if (s.StartsWith("`") && s.EndsWith("`"))
                s = s.Substring(1, s.Length - 2);

            // Converte sequências escapadas (string literal contendo \r\n) para quebras reais
            if (s.Contains("\\r\\n") || s.Contains("\\n") || s.Contains("\\t"))
            {
                s = s.Replace("\\r\\n", "\r\n")
                     .Replace("\\n", "\n")
                     .Replace("\\t", "\t");
            }

            // Remover espaços sobrando no início/fim
            s = s.Trim();

            return s;
        }
    }
}
