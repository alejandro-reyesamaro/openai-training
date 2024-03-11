using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Logging;

namespace ChGPTcmd.Application.Handlers
{
    public class SpeechServiceHandler : IServiceHandler
    {
        private ILogger<SpeechServiceHandler> logger;
        private ISpeechService service;
        private ICommandCompiler compiler;

        public SpeechServiceHandler(ISpeechService service, ICommandCompiler compiler, ILogger<SpeechServiceHandler> logger)
        {
            this.service = service;
            this.compiler = compiler;
            this.logger = logger;
        }

        public bool Handles(int option)
        {
            return option == 3;
        }

        public async Task Handle()
        {
            CommandStatus result = CommandStatus.Success;
            do
            {
                Console.Write(CmdConstants.PROMPT_IMG_TAG);
                Console.WriteLine("Type \"go\" to start recognizing, or \"quit\" to exit");
                string? commandLine = Console.ReadLine();
                PromptResult compilerResult = compiler.ExtractCommand(commandLine ?? CmdConstants.PROMPT_QUIT_COMMAND);
                result = compilerResult.State;

                if (compilerResult != null)
                {
                    if (compilerResult.State == CommandStatus.Success && compilerResult.Prompt == "go")
                    {
                        string speech = await service.RecognizeSpeechAsync();
                        Console.WriteLine(speech);
                    }
                }
                else
                {
                    logger.LogError("Error getting command line!");
                }

            } while (result != CommandStatus.Quit);
        }        
    }
}
