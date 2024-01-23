using ChGPTcmd.Models.ActionResult;
using Microsoft.Extensions.Configuration;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using ChGPTcmd.Infrastructure.DTOs;

namespace ChGPTcmd.Infrastructure.Services
{
    public class HttpOpenAiChatService : BaseChatService, IChatService
    {
        private HttpClient httpClient;
        private string endpoint;
        private ILogger<HttpOpenAiChatService> logger;

        public HttpOpenAiChatService(IConfiguration configuration, ILogger<HttpOpenAiChatService> logger)
        {
            httpClient = new HttpClient();
            string key = configuration.GetValue<string>("OpenAI:ApiKey") ?? throw new InvalidDataException("Failed to load OpenApi-Key");
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {key}");
            endpoint = configuration.GetValue<string>("OpenAI:ChatApiEndPoint") ?? throw new InvalidDataException("Failed to load OpenApi-Endpoint");
            this.logger = logger;
        }
        
        public async Task SetUp(IList<string> systemMessages)
        {
            systemMessages.ToList().ForEach(msg => this.systemMessages.Add(new ChatRequestSystemMessageDto(msg)));
            StringContent content = BuildSystemContent(ModelConstants.MODEL_GPT3Turbo);
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            try
            {
                var data = JsonConvert.DeserializeObject<HttpOpenApiChatResponseDto>(strResponse);
                string? answer = data?.Choices?.ElementAt(0).Message?.Content;
                if (answer == null)
                {
                    logger.LogError(strResponse);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(answer);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"---> Deserialization failed: {ex.Message}");
            }
        }

        public async Task<PromptResult> Post(string prompt)
        {
            historyMessages.Add(new ChatRequestUserMessageDto(prompt));
            StringContent content = BuildChatContent(ModelConstants.MODEL_GPT3Turbo);
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            PromptResult result = new PromptResult();
            try
            {
                var data = JsonConvert.DeserializeObject<HttpOpenApiChatResponseDto>(strResponse);
                if (data == null || data.Choices == null || data.Choices.Count() == 0)
                {
                    result.State = CommandStatus.ApiError;
                    result.MainMessage = "API Error: Not response received";
                }
                else
                {
                    result.State = CommandStatus.Success;
                    string answer = data.Choices.ElementAt(0).Message?.Content ?? "<No Content>";
                    result.MainMessage = answer;
                    historyMessages.Add(new ChatRequestAssistantMessageDto(answer));
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

        public new void ClearChatHistory()
        {
            base.ClearChatHistory();
        }

        private StringContent BuildSystemContent(string model)
        {
            string content = "{\"model\": \"" + model + "\", \"messages\": [" + BuildMessagesList(systemMessages) + "], \"temperature\": 0.5, \"max_tokens\": 1024, \"top_p\": 1, \"frequency_penalty\": 0, \"presence_penalty\": 0 }";
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private StringContent BuildChatContent(string model)
        {
            string content = "{\"model\": \"" + model + "\", \"messages\": [" + BuildMessagesList(historyMessages) + "], \"temperature\": 0.5, \"max_tokens\": 1024, \"top_p\": 1, \"frequency_penalty\": 0, \"presence_penalty\": 0 }";
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private string BuildMessagesList(List<IChatRequestMessage> messages)
        {
            var jsonMessages = new List<string>();
            foreach (var message in messages)
            {
                jsonMessages.Add("{\"role\": \"" + message.Role.ToString() + "\", \"content\": \"" + message.Content + "\"}");
            }

            return string.Join(",", jsonMessages);
        }
    }
}
