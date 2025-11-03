// Import packages 
using DotNetSemanticAgent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Build configuration (Alternative approach without SetBasePath)
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>() // For local development
    .Build();

// Load values from configuration
string modelId = configuration["AzureOpenAI:ModelId"]
    ?? throw new InvalidOperationException("ModelId not configured");
string endpoint = configuration["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("Endpoint not configured");
string apiKey = configuration["AzureOpenAI:ApiKey"]
    ?? throw new InvalidOperationException("ApiKey not configured");

// Create a kernel with Azure OpenAI chat completion 
IKernelBuilder builder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

// Add enterprise components 
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Build the kernel 
Kernel kernel = builder.Build();
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add a plugin (the LightsPlugin class is defined below) 
kernel.Plugins.AddFromType<LightsPlugin>("Lights");

// Enable planning 
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create a history store the conversation 
ChatHistory history = new();

// Initiate a back-and-forth chat 
string? userInput;
do
{
    // Collect user input 
    Console.Write("User > ");
    userInput = Console.ReadLine();

    // Add user input 
    history.AddUserMessage(userInput);

    // Get the response from the AI 
    ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results 
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history 
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);