using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ImageResponseItemDto
    {
        [property: JsonPropertyName("created")]
        public int Created { get; set; }
        
        [property: JsonPropertyName("expires")] 
        public int Expires { get; set; }

        [property: JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [property: JsonPropertyName("result")]
        public ImageResponseResultDto Result { get; set; }

        [property: JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
