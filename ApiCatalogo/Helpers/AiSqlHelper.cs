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
        public static bool IsSafeSelectQuery(string sql)
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
