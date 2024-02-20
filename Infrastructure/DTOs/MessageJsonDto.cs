using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class MessageJsonDto
    {
        [property: JsonPropertyName("role")]
        public string? Role { get; set; }

        [property: JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
