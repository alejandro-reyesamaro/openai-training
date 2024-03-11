namespace ChGPTcmd.Infrastructure.Configuration.Options
{
    public class SpeechOptions
    {
        public static readonly string CONFIG_SECTION = "Speech";

        public string? AzureSpeechServiceKey { get; set; }
        public string? AzureSpeechServiceEndPoint { get; set; }
        public string? AzureSpeechServiceRegion { get; set; }
    }
}
