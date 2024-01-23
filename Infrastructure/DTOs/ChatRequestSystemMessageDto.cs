using Azure.AI.OpenAI;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public class ChatRequestSystemMessageDto : ChatRequestSystemMessage, IChatRequestMessage
    {
        public ChatRequestSystemMessageDto(string content) : base(content) { }

        public ChatRequestMessage OpenAiInstance { get { return this; } }
    }
}
