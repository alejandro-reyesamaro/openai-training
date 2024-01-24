using ChGPTcmd.Application.Services;
using ChGPTcmd.Infrastructure.Configuration.Options;
using ChGPTcmd.Models.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace ChGPTcmd.Infrastructure.Services
{
    public class HttpOpenAiImageGenerationService : ImageGenerationService
    {
        private HttpClient httpClient;
        private OpenAiOptions openAiOptions;
        private ILogger<HttpOpenAiImageGenerationService> logger;

        public HttpOpenAiImageGenerationService(IOptions<OpenAiOptions> openAiOptions, ILogger<HttpOpenAiImageGenerationService> logger)
        {
            this.openAiOptions = openAiOptions.Value;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {this.openAiOptions.ApiKey}");
            this.logger = logger;
        }

        public async Task Post(string prompt)
        {
            StringContent content = BuildBody(prompt);
            HttpResponseMessage response = await httpClient.PostAsync(openAiOptions.ImageApiEndPoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(strResponse);
        }

        // TODO

        private StringContent BuildBody(string prompt)
        {
            string content = "{\"model\": \"" + ModelConstants.MODEL_DALL_E_3 + "\", \"prompt\": \"" + prompt + "\", \"n\": 1, \"size\": \"1024x1024\" }";
            return new StringContent(content, Encoding.UTF8, "application/json");   
        }
    }
}
