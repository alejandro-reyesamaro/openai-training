using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ImageRequestBodyDto
    {
        [property: JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [property: JsonPropertyName("n")] 
        public int? AmountOfImages { get; set; }
        
        [property: JsonPropertyName("size")] 
        public string? ImageSize { get; set; }
    }
}
