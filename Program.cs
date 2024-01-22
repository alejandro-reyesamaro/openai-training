using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Azure.AI.OpenAI;
using System.Text;
using Azure;
using ChGPTcmd.Models.Enums;
using ChGPTcmd.Models.Constants;
using ChGPTcmd.Application;
using ChGPTcmd.Models.ActionResult;
using ChGPTcmd.Infrastructure;
using ChGPTcmd.Infrastructure.Handlers;
using ChGPTcmd.Application.Handlers;
using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Infrastructure.Compilers;

namespace ChGPTcmd.Main
{
    public class Programm
    {
        static async Task Main(string[] args)
        {
            IConfiguration? configuration;

            try
            {
                var builder = new ConfigurationBuilder();
                BuildConfig(builder);
                configuration = builder.Build();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Fatal ERROR while loading appliction settings file!");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return;
            }

            IList<string> messages = new List<string>()
            {
                "I am a highly intelligent question answering bot. If you ask me a question that is rooted in truth, I will give you the answer. If you ask me a question that is nonsense, trickery, or has no clear answer, I will respond with UNKNOWN"
            };

            //!- Dependency injection
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient(c => configuration);
                    services.AddTransient<ICommandCompiler, CommandCompiler>();
                    services.AddTransient<IChatHandler, OpenAiHandler>();
                })
                .UseSerilog()
                .Build();

            // Start 
            var chatHandler = ActivatorUtilities.CreateInstance<OpenAiHandler>(host.Services);
            var compiler = ActivatorUtilities.CreateInstance<CommandCompiler>(host.Services);

            CommandStatus result = CommandStatus.Success;
            await chatHandler.SetUp(messages);
            do
            {
                Console.Write(CmdConstants.PROMPT_TAG);
                string? commandLine = Console.ReadLine();
                PromptResult compilerResult = compiler.ExtractCommand(commandLine ?? CmdConstants.PROMPT_QUIT_COMMAND);
                result = compilerResult.State;

                if (compilerResult != null)
                {
                    if (compilerResult.State == CommandStatus.Success)
                    {
                        PromptResult response = await chatHandler.Handle(compilerResult.Prompt);
                        PrintResponse(response.MainMessage);
                    }
                }
                else
                {
                    Log.Logger.Error("Error getting command line!");
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

        private static void PrintResponses(List<string> messages)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < messages.Count(); i++) 
            {
                Console.WriteLine($"{i+1}- {messages[i]}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"settings.local.json", optional: true)
                .AddJsonFile($"settings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}