using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ChGPTcmd.Infrastructure.Services;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Infrastructure.Compilers;
using ChGPTcmd.Infrastructure.Configuration.Options;
using ChGPTcmd.Application.Handlers;

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

            //!- Dependency injection
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions()
                        .Configure<AzureOpenAiOptions>(configuration.GetSection(AzureOpenAiOptions.CONFIG_SECTION))
                        .Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.CONFIG_SECTION))
                        .Configure<SpeechOptions>(configuration.GetSection(SpeechOptions.CONFIG_SECTION));

                    services.AddTransient(c => configuration);
                    services.AddTransient<ICommandCompiler, CommandCompiler>();
                    services.AddTransient<IServiceHandler, ChatServiceHandler>();
                    services.AddTransient<IServiceHandler, ImageGenerationServiceHandler>();
                    services.AddTransient<IServiceHandler, SpeechServiceHandler>();

                    //services.AddTransient<IChatService, HttpOpenAiChatService>();
                    services.AddTransient<IChatService, AzureOpenAiChatService>();

                    //services.AddTransient<ImageGenerationService, HttpOpenAiImageGenerationService>();
                    services.AddTransient<ImageGenerationService, HttpAzureOpenAiImageGenerationService>();

                    services.AddTransient<ISpeechService, AzureSpeechService>();
                })
                .UseSerilog()
                .Build();

            // Start
            List<IServiceHandler> handlers = new List<IServiceHandler>()
            {
                ActivatorUtilities.CreateInstance<ChatServiceHandler>(host.Services),
                ActivatorUtilities.CreateInstance<ImageGenerationServiceHandler>(host.Services),
                ActivatorUtilities.CreateInstance<SpeechServiceHandler>(host.Services)
            };

            int option = 1;
            do
            {
                Console.WriteLine("Chose a service (enter the number)");
                Console.WriteLine("(0) EXIT");
                Console.WriteLine("(1) ChatGPT Service");
                Console.WriteLine("(2) Image Generation Service");
                Console.WriteLine("(3) Speech Recognition Service");

                string? line = Console.ReadLine();
                bool correct = int.TryParse(line, out option);
                if (correct)
                {
                    foreach (var handler in handlers)
                        if (handler.Handles(option))
                            await handler.Handle();
                }
                else
                {
                    Log.Logger.Error("Error getting option!");
                }

            } while (option != 0);

            Console.WriteLine("Bye!");
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