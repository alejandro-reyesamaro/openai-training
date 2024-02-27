using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ImageResponseDataDto
    {
        [property: JsonPropertyName("url")]
        public string Url { set; get; } = string.Empty;
    }
}
