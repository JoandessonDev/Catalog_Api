namespace ApiCatalogo.Models.DTOs
{
    public class EntityPropertyDTO
    {
        public string Name { get; set; } = default!;
        public string? ColumnName { get; set; }
        public string Type { get; set; } = default!;
        public bool IsPrimaryKey { get; set; }
    }
}
