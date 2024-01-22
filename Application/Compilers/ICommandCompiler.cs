using ChGPTcmd.Models.ActionResult;

namespace ChGPTcmd.Application.Compilers
{
    public interface ICommandCompiler
    {
        public PromptResult ExtractCommand(string commandLine);
    }
}
