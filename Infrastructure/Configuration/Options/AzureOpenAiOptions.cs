namespace ChGPTcmd.Infrastructure.Configuration.Options
{
    public class AzureOpenAiOptions
    {
        public static readonly string CONFIG_SECTION = "AzureOpenAI";

        public string? ApiKey { get; set; }
        public string? ApiEndPoint { get; set; }
        public string? DeploymentName { get; set; }
        public string? ImageGenerationApiKey { get; set; }
        public string? ImageGenerationBaseUrl { get; set; }
        public string? ImageOperationBaseUrl { get; set; }
        public string? ImageGenerationResource { get; set; }
    }
}
