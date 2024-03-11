using ChGPTcmd.Application.Services;
using ChGPTcmd.Infrastructure.Configuration.Options;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;

namespace ChGPTcmd.Infrastructure.Services
{
    public class AzureSpeechService : ISpeechService
    {
        private SpeechOptions Options { get; set; }
        private SpeechConfig Config { get; set; }

        public AzureSpeechService(IOptions<SpeechOptions> speechOptions) 
        {
            Options = speechOptions.Value;
            Config = SpeechConfig.FromSubscription(Options.AzureSpeechServiceKey, Options.AzureSpeechServiceRegion);
        }

        public async Task<string> RecognizeSpeechAsync()
        {
            using var recognizer = new SpeechRecognizer(Config);
            var result = await recognizer.RecognizeOnceAsync();
            var reason = GetRecognitionResultReason(result);
            return reason;
        }

        private string GetRecognitionResultReason(SpeechRecognitionResult result) =>
            result.Reason switch
            {
                ResultReason.RecognizedSpeech => $"Result: {result.Text}",
                ResultReason.NoMatch => "Not recognized",
                ResultReason.Canceled => $"Canceled: {CancellationDetails.FromResult(result).ErrorDetails}",
                _ => $"Unknown reason: {result.Reason}"
            };
    }
}
