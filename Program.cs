﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ChGPTcmd.Infrastructure.Services;
using ChGPTcmd.Application.Services;
using ChGPTcmd.Application.Compilers;
using ChGPTcmd.Infrastructure.Compilers;
using ChGPTcmd.Infrastructure.Handlers;

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
                    services.AddTransient<IServiceHandler, ChatServiceHandler>();
                    services.AddTransient<IChatService, HttpOpenAiChatService>();
                })
                .UseSerilog()
                .Build();

            // Start 
            var handler = ActivatorUtilities.CreateInstance<ChatServiceHandler>(host.Services);

            int option = 1;
            do
            {
                Console.WriteLine("Chose a service (enter the number)");
                Console.WriteLine("(1) ChatGPT Service");

                string? line = Console.ReadLine();
                bool correct = int.TryParse(line, out option);
                if (correct)
                {
                    if (handler.Handles(option))
                        await handler.Handle();
                }
                else
                {
                    Log.Logger.Error("Error getting option!");
                }

            } while (option != 0);

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