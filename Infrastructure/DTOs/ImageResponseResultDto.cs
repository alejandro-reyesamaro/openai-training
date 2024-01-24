using System.Text.Json.Serialization;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ImageResponseResultDto
    {
        [property: JsonPropertyName("data")] 
        public ImageResponseDataDto[] Data { get; set; }
    }
}
