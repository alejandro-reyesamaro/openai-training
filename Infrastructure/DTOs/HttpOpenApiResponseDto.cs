namespace ChGPTcmd.Infrastructure.DTOs
{
    public class HttpOpenApiResponseDto
    {
        public string? Id { get; set; }
        public string? Object { get; set; }
        public string? Model { get; set; }
        public IEnumerable<ChoiceJsonDto>? Choices { get; set; }
    }
}
