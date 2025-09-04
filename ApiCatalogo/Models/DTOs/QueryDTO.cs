namespace ApiCatalogo.Models.DTOs
{
    public class QueryDTO
    {
        public string Type { get; set; }
        public string Entity { get; set; }
        public string Filter { get; set; }
        public List<object> Parameters { get; set; } = new();
        public string OrderBy { get; set; }
        public int? Limit { get; set; }
        public List<string> TablesUsed { get; set; } = new();
        public List<string> ColumnsUsed { get; set; } = new();
    }

}
