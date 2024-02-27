using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Logging;

namespace ChGPTcmd.Application.Handlers
{
    public class ChatServiceHandler : IServiceHandler
    {
        private ILogger<ChatServiceHandler> logger;
        private IChatService chatService;
        private ICommandCompiler compiler;

        public ChatServiceHandler(IChatService chatService, ICommandCompiler compiler, ILogger<ChatServiceHandler> logger)
        {
            this.chatService = chatService;
            this.compiler = compiler;
            this.logger = logger;
        }

        public bool Handles(int option)
        {
            return option == 1;
        }

        public async Task Handle()
        {
            IList<string> messages = new List<string>()
            {
                "You are a highly intelligent question answering bot. If I ask you a question that is rooted in truth, you will give me the answer. If I ask you a question that is nonsense, trickery, or has no clear answer, you will respond with UNKNOWN"
            };

            CommandStatus result = CommandStatus.Success;
            await chatService.SetUp(messages);

            do
            {
                Console.Write(CmdConstants.PROMPT_CHAT_TAG);
                string? commandLine = Console.ReadLine();
                PromptResult compilerResult = compiler.ExtractCommand(commandLine ?? CmdConstants.PROMPT_QUIT_COMMAND);
                result = compilerResult.State;

                if (compilerResult != null)
                {
                    if (compilerResult.State == CommandStatus.Success)
                    {
                        PromptResult response = await chatService.Post(compilerResult.Prompt);
                        PrintResponse(response.MainMessage);
                    }
                }
                else
                {
                    logger.LogError("Error getting command line!");
                }

            } while (result != CommandStatus.Quit);
        }

        private static void PrintResponse(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"R/ {message}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}
