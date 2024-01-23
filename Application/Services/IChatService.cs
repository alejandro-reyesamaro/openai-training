using ChGPTcmd.Models.ActionResult;

namespace ChGPTcmd.Application.Services

{
    public interface IChatService
    {
        public Task SetUp(IList<string> systemMessages);
        public Task<PromptResult> Post(string prompt);
        public void ClearChatHistory();
    }
}
