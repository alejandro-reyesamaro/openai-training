using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ImageRequestWithModelBodyDto : ImageRequestBodyDto
    {
        [property: JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;
    }
}
