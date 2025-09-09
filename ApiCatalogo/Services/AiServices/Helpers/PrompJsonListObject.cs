using System.Collections;
using System.Text.Json;

namespace ApiCatalogo.Services.AiServices.Helpers
{
    public static class PrompJsonListObject
    {
        public static string PrepareListJsonForPrompt(object listObject, int maxItems = 1000)
        {
            if (listObject == null)
                return "[]";

            if (listObject is string s && IsJson(s))
                return EscapeForScript(s);

            try
            {
                if (listObject is IEnumerable<object> genericEnumerable)
                {
                    var items = genericEnumerable.ToList();
                    return EscapeForScript(SerializeWithSampling(items, maxItems));
                }

                if (listObject is IEnumerable nonGeneric)
                {
                    var items = nonGeneric.Cast<object>().ToList();
                    return EscapeForScript(SerializeWithSampling(items, maxItems));
                }

                return EscapeForScript(JsonSerializer.Serialize(listObject));
            }
            catch
            {
                return EscapeForScript(JsonSerializer.Serialize(listObject.ToString()));
            }
        }


        private static string SerializeWithSampling(List<object> items, int maxItems)
        {
            if (items == null || items.Count == 0)
                return "[]";


            if (items.Count <= maxItems)
                return JsonSerializer.Serialize(items);


            var sampled = new List<object> { items.First() };
            int middleCount = Math.Max(0, maxItems - 2); 
            double step = (double)(items.Count - 2) / Math.Max(1, middleCount);


            for (int i = 0; i < middleCount; i++)
            {
                int index = 1 + (int)Math.Round(i * step);
                index = Math.Min(index, items.Count - 2);
                sampled.Add(items[index]);
            }


            sampled.Add(items.Last());
            return JsonSerializer.Serialize(sampled);
        }


        private static bool IsJson(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;


            str = str.Trim();
            return (str.StartsWith("{") && str.EndsWith("}")) || (str.StartsWith("[") && str.EndsWith("]"));
        }


        private static string EscapeForScript(string json)
        {
            if (json == null)
                return "null";

            return json.Replace("</script>", "<\\/script>");
        }
    }
}
