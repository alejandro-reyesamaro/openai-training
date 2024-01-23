using Azure.AI.OpenAI;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ChatRequestUserMessageDto : ChatRequestUserMessage, IChatRequestMessage 
    { 
        public ChatRequestUserMessageDto(string content) : base(content) { }

        public ChatRequestMessage OpenAiInstance { get { return this; } }
    }
}
