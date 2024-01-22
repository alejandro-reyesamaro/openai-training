using ChGPTcmd.Models.ActionResult;

namespace ChGPTcmd.Application.Services

{
    public interface IChatService
    {
        public Task SetUp(IList<string> systemMessages);
        public Task<PromptResult> Handle(string prompt);
    }
}
