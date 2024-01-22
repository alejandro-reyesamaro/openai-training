using Azure.AI.OpenAI;
using ChGPTcmd.Models.ActionResult;

namespace ChGPTcmd.Application.Handlers
{
    public interface IChatHandler
    {
        public Task SetUp(IList<string> systemMessages);
        public Task<PromptResult> Handle(string prompt);
    }
}
