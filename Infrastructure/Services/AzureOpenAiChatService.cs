using Azure;
using Azure.AI.OpenAI;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Configuration;

namespace ChGPTcmd.Infrastructure.Services
{
    public class AzureOpenAiChatService : IChatService
    {
        private OpenAIClient client;
        private string deploymentName;
        private ChatCompletionsOptions options;

        public AzureOpenAiChatService(IConfiguration configuration)
        {
            client = new OpenAIClient(
                    new Uri(configuration.GetValue<string>("AzureOpenAI:ApiEndPoint") ?? throw new InvalidDataException("Failed to load Azure OpenApi-Endpoint")),
                    new AzureKeyCredential(configuration.GetValue<string>("AzureOpenAI:ApiKey") ?? throw new InvalidDataException("Failed to load Azure OpenApi-key")));
            deploymentName = configuration.GetValue<string>("AzureOpenAI:DeploymentName") ?? throw new InvalidDataException("Failed to load Azure OpenApi deployment name");
            options = new ChatCompletionsOptions()
            {
                Temperature = (float)0.7,
                MaxTokens = 800,
                DeploymentName = deploymentName,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };
        }

        public Task SetUp(IList<string> systemMessages)
        {
            systemMessages.ToList().ForEach(msg => options.Messages.Add(new ChatRequestSystemMessage(msg)));
            return Task.CompletedTask;
        }

        public async Task<PromptResult> Handle(string prompt)
        {
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
                result.MainMessage = response.Choices.ElementAt(0).Message.Content;
            }
            return result;
        }
    }
}
