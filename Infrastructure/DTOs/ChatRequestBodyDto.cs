using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ChatRequestBodyDto
    {
        [property: JsonPropertyName("model")]
        public string Model { get; set; }

        [property: JsonPropertyName("messages")]
        public IEnumerable<MessageJsonDto> Messages { get; set; }

        [property: JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [property: JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [property: JsonPropertyName("top_p")]
        public int TopP { get; set; }

        [property: JsonPropertyName("frequency_penalty")]
        public int FrequencyPenalty { get; set; }

        [property: JsonPropertyName("presence_penalty")]
        public int PresencePenalty { get; set; }
    }
}
