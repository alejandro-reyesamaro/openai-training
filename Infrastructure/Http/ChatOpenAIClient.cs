using ChGPTcmd.Infrastructure.Configuration.Options;

namespace ChGPTcmd.Infrastructure.Http
{
    public class ChatOpenAIClient : HttpClient
    {
        protected OpenAiOptions options;

        public ChatOpenAIClient(OpenAiOptions options) 
        {
            this.options = options;
            this.DefaultRequestHeaders.Add("authorization", $"Bearer {options.ApiKey}");
        }

        public async Task<HttpResponseMessage> PostAsync(HttpContent? content) =>
            await PostAsync(options.ChatApiEndPoint, content);
    }
}
