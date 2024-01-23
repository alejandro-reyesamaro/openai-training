using Azure.AI.OpenAI;

namespace ChGPTcmd.Infrastructure.DTOs
{
    public interface IChatRequestMessage
    {
        public ChatRole Role { get; }
        public string Content { get; }

        public ChatRequestMessage OpenAiInstance { get; }
    }
}
