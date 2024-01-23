using Azure.AI.OpenAI;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ChatRequestAssistantMessageDto : ChatRequestAssistantMessage, IChatRequestMessage
    {
        public ChatRequestAssistantMessageDto(string content) : base(content) { }

        public ChatRequestMessage OpenAiInstance { get { return this; } }
    }
}
