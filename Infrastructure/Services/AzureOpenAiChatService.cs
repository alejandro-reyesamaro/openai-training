using Azure;
using Azure.AI.OpenAI;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Infrastructure.Configuration.Options;
using ChGPTcmd.Infrastructure.DTOs;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace ChGPTcmd.Infrastructure.Services
{
    public class AzureOpenAiChatService : BaseChatService, IChatService
    {
        private OpenAIClient client;
        private AzureOpenAiOptions openAiOptions;
        private ChatCompletionsOptions options;

        public AzureOpenAiChatService(IOptions<AzureOpenAiOptions> openAiOptions)
        {
            this.openAiOptions = openAiOptions.Value;
            client = new OpenAIClient(
                    new Uri(this.openAiOptions.ApiEndPoint ?? throw new InvalidDataException("Failed to load Azure OpenApi-Endpoint")),
                    new AzureKeyCredential(this.openAiOptions.ApiKey ?? throw new InvalidDataException("Failed to load Azure OpenApi-key")));
            SetUpOptions();
        }

        public Task SetUp(IList<string> systemMessages)
        {
            systemMessages.ToList().ForEach(msg => this.systemMessages.Add(new ChatRequestSystemMessageDto(msg)));
            systemMessages.ToList().ForEach(msg => options.Messages.Add(new ChatRequestSystemMessage(msg)));
            return Task.CompletedTask;
        }

        public async Task<PromptResult> Post(string prompt)
        {
            options.Messages.Add(new ChatRequestUserMessage(prompt));
            historyMessages.Add(new ChatRequestUserMessageDto(prompt));
            Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(options);
            ChatCompletions response = responseWithoutStream.Value;
            PromptResult result = new();

            if (response.Choices == null || response.Choices.Count == 0)
            {
                result.State = CommandStatus.ApiError;
            }
            else
            {
                result.State = CommandStatus.Success;
                string answer = response.Choices.ElementAt(0).Message.Content;
                result.MainMessage = answer;
                options.Messages.Add(new ChatRequestAssistantMessage(answer));
                historyMessages.Add(new ChatRequestAssistantMessageDto(answer));
            }
            return result;
        }

        public new void ClearChatHistory()
        {
            base.ClearChatHistory();
            SetUpOptions();
            systemMessages.ToList().ForEach(msg => options.Messages.Add(new ChatRequestSystemMessage(msg.Content)));
        }

        [MemberNotNull(nameof(options))]
        private void SetUpOptions()
        {
            options = new ChatCompletionsOptions()
            {
                Temperature = (float)0.7,
                MaxTokens = 800,
                DeploymentName = openAiOptions.DeploymentName ?? throw new InvalidDataException("Failed to load Azure OpenApi deployment name"),                
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };
        }
    }
}
