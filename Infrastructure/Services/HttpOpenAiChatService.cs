using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Logging;
using System.Text;
using ChGPTcmd.Infrastructure.DTOs;
using ChGPTcmd.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ChGPTcmd.Infrastructure.Services
{
    public class HttpOpenAiChatService : BaseChatService, IChatService
    {
        private HttpClient httpClient;
        private OpenAiOptions openAiOptions;
        private ILogger<HttpOpenAiChatService> logger;

        public HttpOpenAiChatService(IOptions<OpenAiOptions> openAiOptions, ILogger<HttpOpenAiChatService> logger)
        {
            this.openAiOptions = openAiOptions.Value;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {this.openAiOptions.ApiKey}");
            this.logger = logger;
        }
        
        public async Task SetUp(IList<string> systemMessages)
        {
            systemMessages.ToList().ForEach(msg => this.systemMessages.Add(new ChatRequestSystemMessageDto(msg)));
            StringContent content = BuildSystemContent(ModelConstants.MODEL_GPT3Turbo);
            HttpResponseMessage response = await httpClient.PostAsync(openAiOptions.ChatApiEndPoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            try
            {
                var data = JsonSerializer.Deserialize<HttpOpenApiChatResponseDto>(strResponse);
                string? answer = data?.Choices?.ElementAt(0).Message?.Content;
                if (answer == null)
                {
                    logger.LogError(strResponse);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("System message:");
                    foreach (string sysMsg in systemMessages)
                    {
                        Console.WriteLine(sysMsg);
                    }
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(answer);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Deserialization failed: {ex.Message}");
            }
        }

        public async Task<PromptResult> Post(string prompt)
        {
            historyMessages.Add(new ChatRequestUserMessageDto(prompt));
            StringContent content = BuildChatContent(ModelConstants.MODEL_GPT3Turbo);
            HttpResponseMessage response = await httpClient.PostAsync(openAiOptions.ChatApiEndPoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            PromptResult result = new PromptResult();
            try
            {
                var data = JsonSerializer.Deserialize<HttpOpenApiChatResponseDto>(strResponse);
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
                logger.LogError($"Deserialization failed: {ex.Message}");
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
            return BuildContent(model, systemMessages);
        }

        private StringContent BuildChatContent(string model)
        {
            return BuildContent(model, historyMessages);
        }

        private StringContent BuildContent(string model, List<IChatRequestMessage> messages)
        {
            List<MessageJsonDto> jsonMessages = messages.Select(m => {
                return new MessageJsonDto
                {
                    Role = m.Role.ToString(),
                    Content = m.Content
                };
            }).ToList();

            var request = new ChatRequestBodyDto
            {
                Model = model,
                Messages = jsonMessages,
                Temperature = 0.5,
                MaxTokens = 1024,
                TopP = 1,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            string content = string.Empty;
            try
            {
                content = JsonSerializer.Serialize(request);
            }
            catch (Exception ex)
            {
                logger.LogError($"Internal deserialization failed: {ex.Message}");
            }

            return new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
