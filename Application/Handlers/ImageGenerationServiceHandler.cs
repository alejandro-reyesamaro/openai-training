using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Models.Enums;
using Microsoft.Extensions.Logging;

namespace ChGPTcmd.Application.Handlers
{
    public class ImageGenerationServiceHandler : IServiceHandler
    {
        private ILogger<ImageGenerationServiceHandler> logger;
        private ImageGenerationService service;
        private ICommandCompiler compiler;

        public ImageGenerationServiceHandler(ImageGenerationService service, ICommandCompiler compiler, ILogger<ImageGenerationServiceHandler> logger)
        {
            this.service = service;
            this.compiler = compiler;
            this.logger = logger;
        }

        public bool Handles(int option)
        {
            return option == 2;
        }

        public async Task Handle()
        {
            CommandStatus result = CommandStatus.Success;
            do
            {
                Console.Write(CmdConstants.PROMPT_IMG_TAG);
                string? commandLine = Console.ReadLine();
                PromptResult compilerResult = compiler.ExtractCommand(commandLine ?? CmdConstants.PROMPT_QUIT_COMMAND);
                result = compilerResult.State;

                if (compilerResult != null)
                {
                    if (compilerResult.State == CommandStatus.Success)
                    {
                        await service.Post(compilerResult.Prompt);
                        Console.WriteLine("Image generated");
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
