using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AZ204.Integration;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                });
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("?   AZ-204 Module 5: Connect to and Consume Azure Services    ?");
        Console.WriteLine("?   Interactive Learning Environment                           ?");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine();
        Console.WriteLine("?? Coming Soon!");
        Console.WriteLine();
        Console.WriteLine("This module will cover:");
        Console.WriteLine("  • Azure API Management (policies, products)");
        Console.WriteLine("  • Azure Event Grid (custom topics, event filtering)");
        Console.WriteLine("  • Azure Event Hubs (producers, consumers, checkpointing)");
        Console.WriteLine("  • Azure Service Bus (topics, sessions, transactions)");
        Console.WriteLine("  • Azure Logic Apps (workflows, connectors)");
        Console.WriteLine();
        Console.WriteLine("?? Exam Weight: 15-20%");
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        await Task.CompletedTask;
    }
}
