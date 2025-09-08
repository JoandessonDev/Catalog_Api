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


            if ((s.Length >= 2) && ((s[0] == '"' && s[^1] == '"') || (s[0] == '\'' && s[^1] == '\'')))
                s = s.Substring(1, s.Length - 2).Trim();


            s = Regex.Replace(s, @"^```[a-zA-Z]*\r?\n", string.Empty, RegexOptions.IgnoreCase); 
            s = Regex.Replace(s, @"\r?\n```$", string.Empty, RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"^```$", string.Empty, RegexOptions.IgnoreCase);

            s = Regex.Replace(s, @"^\s*(sql|SQL)\s*[:\-]*\s*\r?\n", string.Empty, RegexOptions.IgnoreCase);

            if (s.StartsWith("`") && s.EndsWith("`"))
                s = s.Substring(1, s.Length - 2);

            if (s.Contains("\\r\\n") || s.Contains("\\n") || s.Contains("\\t"))
            {
                s = s.Replace("\\r\\n", "\r\n")
                     .Replace("\\n", "\n")
                     .Replace("\\t", "\t");
            }

            s = s.Trim();

            return s;
        }
    }
}
