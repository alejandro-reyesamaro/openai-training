using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;

namespace ChGPTcmd.Infrastructure.Compilers
{
    public class CommandCompiler : ICommandCompiler
    {
        public PromptResult ExtractCommand(string commandLine)
        {
            return CmdConstants.PROMPT_QUIT_COMMAND.Equals(commandLine)
                ? new PromptResult() { State = CommandStatus.Quit, Prompt = string.Empty }
                : new PromptResult() { Prompt = commandLine, MainMessage = string.Empty, State = CommandStatus.Success };
        }
    }
}
