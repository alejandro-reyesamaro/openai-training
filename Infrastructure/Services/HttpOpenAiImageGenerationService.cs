using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChGPTcmd.Infrastructure.Services
{
    public class HttpOpenAiImageGenerationService : ImageGenerationService
    {
        private HttpClient httpClient;
        private string endpoint;
        private ILogger<HttpOpenAiImageGenerationService> logger;

        public HttpOpenAiImageGenerationService(IConfiguration configuration, ILogger<HttpOpenAiImageGenerationService> logger)
        {
            httpClient = new HttpClient();
            string key = configuration.GetValue<string>("OpenAI:ApiKey") ?? throw new InvalidDataException("Failed to load OpenApi-Key");
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {key}");
            endpoint = configuration.GetValue<string>("OpenAI:ImageApiEndPoint") ?? throw new InvalidDataException("Failed to load OpenApi-Endpoint");
            this.logger = logger;
        }

        public async Task Post(string prompt)
        {
            StringContent content = BuildBody(prompt);
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(strResponse);
        }

        private StringContent BuildBody(string prompt)
        {
            string content = "{\"model\": \"" + ModelConstants.MODEL_DALL_E_3 + "\", \"prompt\": \"" + prompt + "\", \"n\": 1, \"size\": \"1024x1024\" }";
            return new StringContent(content, Encoding.UTF8, "application/json");   
        }
    }
}
