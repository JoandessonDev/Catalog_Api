namespace ApiCatalogo.Models.DTOs
{
    public class EntitySchemaDTO
    {
        public string TableName { get; set; } = default!;
        public string? ClrType { get; set; }
        public string EntityName { get; set; } = default!;
        public List<EntityPropertyDTO> Properties { get; set; } = new();
    }
}
