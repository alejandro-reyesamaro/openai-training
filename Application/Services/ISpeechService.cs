namespace ChGPTcmd.Application.Services
{
    public interface ISpeechService
    {
        public Task<string> RecognizeSpeechAsync();
    }
}
