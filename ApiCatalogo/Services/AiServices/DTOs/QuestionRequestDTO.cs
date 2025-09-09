using ApiCatalogo.Services.AiServices.Enums;

namespace ApiCatalogo.Services.AiServices.AiServices.DTOs
{
    public class QuestionRequestDTO
    {
        public ModePrompt ModePrompt { get; set; } 
        public string MessageUser { get; set; }
        public List<object>? ListObject { get; set; }

    }
}
