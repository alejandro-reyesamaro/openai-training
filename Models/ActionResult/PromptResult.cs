using ChGPTcmd.Models.Enums;

namespace ChGPTcmd.Models.ActionResult
{
    public class PromptResult
    {
        public string Prompt { get; set; } = string.Empty;
        public string MainMessage { get; set; } = string.Empty;
        public CommandStatus State { get; set; }
    }
}
