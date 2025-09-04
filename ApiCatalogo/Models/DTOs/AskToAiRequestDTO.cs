namespace ApiCatalogo.Models.DTOs
{
    public class AskToAiRequestDTO
    {
        public List<EntitySchemaDTO> Schema { get; set; } = new();
        public string Question { get; set; } = string.Empty;       
    }
}
