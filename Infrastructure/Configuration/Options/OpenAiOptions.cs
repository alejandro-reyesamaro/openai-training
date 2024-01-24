namespace ChGPTcmd.Infrastructure.Configuration.Options
{
    public class OpenAiOptions
    {
        public static readonly string CONFIG_SECTION = "OpenAI";

        public string? ApiKey { get; set; }
        public string? ChatApiEndPoint { get; set; }
        public string? ImageApiEndPoint { get;}
    }
}
