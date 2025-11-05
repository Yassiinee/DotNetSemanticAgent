using DotNetSemanticAgent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = BuildConfiguration();
        var kernel = BuildKernel(configuration);
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var history = new ChatHistory();
        var openAIPromptExecutionSettings = BuildExecutionSettings();

        await StartChatLoopAsync(chatCompletionService, history, openAIPromptExecutionSettings);
    }

    // Build configuration settings from various sources (appsettings, environment, etc.)
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>() // For local development
            .Build();
    }

    // Create and configure the kernel
    private static IKernel BuildKernel(IConfiguration configuration)
    {
        string modelId = configuration["AzureOpenAI:ModelId"]
            ?? throw new InvalidOperationException("ModelId not configured");
        string endpoint = configuration["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException("Endpoint not configured");
        string apiKey = configuration["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException("ApiKey not configured");

        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

        var logLevelString = configuration["Logging:LogLevel"]
            ?? Environment.GetEnvironmentVariable("LOG_LEVEL")
            ?? "Information";

        if (!Enum.TryParse<LogLevel>(logLevelString, ignoreCase: true, out var minLogLevel))
        {
            minLogLevel = LogLevel.Information; // Default to Information if parsing fails
        }

        builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(minLogLevel));

        builder.Plugins.AddFromType<LightsPlugin>("Lights"); // Add the plugin

        return builder.Build();
    }

    // Configure execution settings for OpenAI
    private static OpenAIPromptExecutionSettings BuildExecutionSettings()
    {
        return new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
    }

    // Main chat loop for interacting with the assistant
    private static async Task StartChatLoopAsync(IChatCompletionService chatCompletionService, ChatHistory history, OpenAIPromptExecutionSettings openAIPromptExecutionSettings)
    {
        string? userInput;
        do
        {
            // Collect user input
            Console.Write("User > ");
            userInput = Console.ReadLine();

            if (userInput == null) break;

            // Add user input to the chat history
            history.AddUserMessage(userInput);

            // Get the assistant's response
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings: openAIPromptExecutionSettings);

            // Print assistant's response
            Console.WriteLine("Assistant > " + result);

            // Add the assistant's message to the chat history
            history.AddMessage(result.Role, result.Content ?? string.Empty);

        } while (userInput is not null); // Continue until user exits
    }
}
