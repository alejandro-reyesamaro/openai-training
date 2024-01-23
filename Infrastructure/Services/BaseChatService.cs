using ChGPTcmd.Infrastructure.DTOs;

namespace ChGPTcmd.Infrastructure.Services
{
    public abstract class BaseChatService
    {
        protected List<IChatRequestMessage> systemMessages;
        protected List<IChatRequestMessage> historyMessages;

        protected BaseChatService() 
        { 
            systemMessages = new List<IChatRequestMessage>();
            historyMessages = new List<IChatRequestMessage>();
        }

        protected void ClearChatHistory()
        {
            historyMessages.Clear();
        }
    }
}
