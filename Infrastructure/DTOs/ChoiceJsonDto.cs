using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ChoiceJsonDto
    {
        [property: JsonPropertyName("index")]
        public int Index { get; set; }

        [property: JsonPropertyName("message")]
        public MessageJsonDto? Message { get; set; }
    }
}
