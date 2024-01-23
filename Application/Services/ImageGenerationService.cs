namespace ChGPTcmd.Application.Services
{
    public interface ImageGenerationService
    {
        public Task Post(string prompt);
    }
}
