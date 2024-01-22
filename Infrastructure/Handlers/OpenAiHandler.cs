using ChGPTcmd.Models.ActionResult;
using Microsoft.Extensions.Configuration;
using ChGPTcmd.Application.Handlers;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using ChGPTcmd.Infrastructure.DTOs;

namespace ChGPTcmd.Infrastructure.Handlers
{
    public class OpenAiHandler : IChatHandler
    {
        private HttpClient httpClient;
        private string endpoint;
        private ILogger<OpenAiHandler> logger;

        public OpenAiHandler(IConfiguration configuration, ILogger<OpenAiHandler> logger)
        {
            httpClient = new HttpClient();
            string key = configuration.GetValue<string>("OpenAI:ApiKey") ?? throw new InvalidDataException("Failed to load OpenApi-Key");
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {key}");
            endpoint = configuration.GetValue<string>("OpenAI:ApiEndPoint") ?? throw new InvalidDataException("Failed to load OpenApi-Endpoint");
            this.logger = logger;
        }
        
        public async Task SetUp(IList<string> systemMessages)
        {
            StringContent content = BuildContent(ModelConstants.MODEL_GPT3Turbo, ModelConstants.ROLE_USER, systemMessages.ToArray());
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            try
            {
                var data = JsonConvert.DeserializeObject<HttpOpenApiResponseDto>(strResponse);
                logger.LogInformation(data?.Choices?.ElementAt(0).Message?.Content ?? "ERR");
            }
            catch (Exception ex)
            {
                logger.LogError($"---> Deserialization failed: {ex.Message}");
            }
        }

        public async Task<PromptResult> Handle(string prompt)
        {
            StringContent content = BuildContent(ModelConstants.MODEL_GPT3Turbo, ModelConstants.ROLE_USER, prompt);
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            PromptResult result = new PromptResult();
            try
            {
                var data = JsonConvert.DeserializeObject<HttpOpenApiResponseDto>(strResponse);
                if (data == null || data.Choices == null || data.Choices.Count() == 0)
                {
                    result.State = CommandStatus.ApiError;
                    result.MainMessage = "API Error: Not response received";
                }
                else
                {
                    result.State = CommandStatus.Success;
                    result.MainMessage = data.Choices.ElementAt(0).Message?.Content ?? "<No Content>";
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"---> Deserialization failed: {ex.Message}");
                result.State = CommandStatus.InternalError;
                result.MainMessage = "Exception: " + ex.Message;
            }
            return result;
        }

        private StringContent BuildContent(string model, string role, params string[] messages)
        {
            string content = "{\"model\": \"" + model + "\", \"messages\": [" + BuildMessagesList(role, messages) + "], \"temperature\": 0.5, \"max_tokens\": 1024, \"top_p\": 1, \"frequency_penalty\": 0, \"presence_penalty\": 0 }";
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private string BuildMessagesList(string role, IList<string> messages)
        {
            var jsonMessages = new List<string>();
            foreach (var message in messages)
            {
                jsonMessages.Add("{\"role\": \"" + role + "\", \"content\": \"" + message + "\"}");
            }

            return string.Join(",", jsonMessages);
        }
    }
}
