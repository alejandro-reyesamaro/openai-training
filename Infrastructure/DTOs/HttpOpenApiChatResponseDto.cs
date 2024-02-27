using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class HttpOpenApiChatResponseDto
    {
        [property: JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [property: JsonPropertyName("object")]
        public string? Object { get; set; }

        [property: JsonPropertyName("model")]
        public string? Model { get; set; }

        [property: JsonPropertyName("choices")]
        public IEnumerable<ChoiceJsonDto>? Choices { get; set; }
    }
}
