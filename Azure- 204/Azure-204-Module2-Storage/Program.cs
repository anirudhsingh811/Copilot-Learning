using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;

namespace AZ204.Storage;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddUserSecrets<Program>(optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                });

                // Configure Cosmos DB client
                var cosmosConnectionString = context.Configuration["CosmosDB:ConnectionString"];
                if (!string.IsNullOrEmpty(cosmosConnectionString))
                {
                    services.AddSingleton(new CosmosClient(cosmosConnectionString));
                }

                // Configure Blob Storage client
                var blobConnectionString = context.Configuration["BlobStorage:ConnectionString"];
                if (!string.IsNullOrEmpty(blobConnectionString))
                {
                    services.AddSingleton(new BlobServiceClient(blobConnectionString));
                }
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var configuration = host.Services.GetRequiredService<IConfiguration>();

        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("?   AZ-204 Module 2: Develop for Azure Storage                ?");
        Console.WriteLine("?   Interactive Learning Environment                           ?");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine();

        while (true)
        {
            DisplayMainMenu();
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        await DisplayCosmosDBExamples(logger);
                        break;
                    case "2":
                        await DisplayBlobStorageExamples(logger);
                        break;
                    case "3":
                        await DisplayTableStorageExamples(logger);
                        break;
                    case "4":
                        await DisplayQueueStorageExamples(logger);
                        break;
                    case "5":
                        DisplayStorageDecisionTree();
                        break;
                    case "6":
                        DisplayBestPractices();
                        break;
                    case "7":
                        DisplayExamTips();
                        break;
                    case "0":
                        Console.WriteLine("\n? Goodbye! Good luck with AZ-204!");
                        return;
                    default:
                        Console.WriteLine("\n? Invalid choice. Please try again.\n");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred");
                Console.WriteLine($"\n? Error: {ex.Message}\n");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("\n???????????????????????????????????????????????????????????????");
        Console.WriteLine("?  MAIN MENU - Select a topic:                                ?");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("?  1. Azure Cosmos DB (Partitioning, Consistency, RU)         ?");
        Console.WriteLine("?  2. Azure Blob Storage (SAS, Tiers, Lifecycle)              ?");
        Console.WriteLine("?  3. Azure Table Storage (NoSQL key-value)                   ?");
        Console.WriteLine("?  4. Azure Queue Storage (Message queuing)                   ?");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("?  5. Storage Decision Tree (When to use what?)                ?");
        Console.WriteLine("?  6. Best Practices & Optimization                            ?");
        Console.WriteLine("?  7. AZ-204 Exam Tips                                         ?");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("?  0. Exit                                                     ?");
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.Write("\nEnter your choice: ");
    }

    static async Task DisplayCosmosDBExamples(ILogger<Program> logger)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZURE COSMOS DB - INTERACTIVE DEMOS");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        while (true)
        {
            Console.WriteLine("\n???????????????????????????????????????????????????????????????");
            Console.WriteLine("?  COSMOS DB SCENARIOS:                                        ?");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.WriteLine("?  1. E-commerce Partitioning Strategy                         ?");
            Console.WriteLine("?  2. Consistency Levels Explained                             ?");
            Console.WriteLine("?  3. Change Feed Pattern                                      ?");
            Console.WriteLine("?  4. Request Unit (RU) Optimization                           ?");
            Console.WriteLine("?  5. Global Distribution Setup                                ?");
            Console.WriteLine("?  6. Stored Procedures (Transactions)                         ?");
            Console.WriteLine("?                                                              ?");
            Console.WriteLine("?  0. Back to Main Menu                                        ?");
            Console.WriteLine("???????????????????????????????????????????????????????????????");
            Console.Write("\nEnter your choice: ");

            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        var result1 = await CosmosDB.Examples.CosmosDbScenarios.EcommercePartitioningDemo();
                        Console.WriteLine($"\n?? Result:\n{result1}");
                        break;
                    case "2":
                        var result2 = await CosmosDB.Examples.CosmosDbScenarios.ConsistencyLevelsDemo();
                        Console.WriteLine($"\n?? Result:\n{result2}");
                        break;
                    case "3":
                        var result3 = await CosmosDB.Examples.CosmosDbScenarios.ChangeFeedPatternDemo();
                        Console.WriteLine($"\n?? Result:\n{result3}");
                        break;
                    case "4":
                        var result4 = await CosmosDB.Examples.CosmosDbScenarios.RUOptimizationDemo();
                        Console.WriteLine($"\n?? Result:\n{result4}");
                        break;
                    case "5":
                        var result5 = await CosmosDB.Examples.CosmosDbScenarios.GlobalDistributionDemo();
                        Console.WriteLine($"\n?? Result:\n{result5}");
                        break;
                    case "6":
                        var result6 = await CosmosDB.Examples.CosmosDbScenarios.StoredProcedureDemo();
                        Console.WriteLine($"\n?? Result:\n{result6}");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n? Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Cosmos DB demo");
                Console.WriteLine($"\n? Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    static async Task DisplayBlobStorageExamples(ILogger<Program> logger)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZURE BLOB STORAGE - INTERACTIVE DEMOS");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Topics covered:");
        Console.WriteLine("   • Upload/Download operations");
        Console.WriteLine("   • SAS (Shared Access Signatures)");
        Console.WriteLine("   • Access tiers (Hot, Cool, Archive)");
        Console.WriteLine("   • Lifecycle management");
        Console.WriteLine("   • Static website hosting");
        Console.WriteLine("   • Blob versioning and soft delete\n");

        Console.WriteLine("?? Coming in next update with full interactive demos!");
        await Task.CompletedTask;
    }

    static async Task DisplayTableStorageExamples(ILogger<Program> logger)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZURE TABLE STORAGE - INTERACTIVE DEMOS");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Topics covered:");
        Console.WriteLine("   • Entity operations (CRUD)");
        Console.WriteLine("   • Partition key design");
        Console.WriteLine("   • Batch operations");
        Console.WriteLine("   • Query optimization\n");

        Console.WriteLine("?? Coming in next update!");
        await Task.CompletedTask;
    }

    static async Task DisplayQueueStorageExamples(ILogger<Program> logger)
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZURE QUEUE STORAGE - INTERACTIVE DEMOS");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Topics covered:");
        Console.WriteLine("   • Message operations");
        Console.WriteLine("   • Visibility timeout");
        Console.WriteLine("   • Poison message handling");
        Console.WriteLine("   • Queue monitoring\n");

        Console.WriteLine("?? Coming in next update!");
        await Task.CompletedTask;
    }

    static void DisplayStorageDecisionTree()
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  STORAGE DECISION TREE");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("? Which storage service should I use?\n");

        Console.WriteLine("?? Need structured data with complex queries?");
        Console.WriteLine("?  ?? YES ? Cosmos DB (globally distributed, multi-model)\n");

        Console.WriteLine("?? Need to store files/documents/media?");
        Console.WriteLine("?  ?? YES ? Blob Storage\n");

        Console.WriteLine("?? Need simple key-value NoSQL?");
        Console.WriteLine("?  ?? YES ? Table Storage (cheap, simple)\n");

        Console.WriteLine("?? Need asynchronous message processing?");
        Console.WriteLine("?  ?? YES ? Queue Storage (simple) or Service Bus (advanced)\n");

        Console.WriteLine("?? Need file shares?");
        Console.WriteLine("?  ?? YES ? Azure Files\n");

        Console.WriteLine("\n?? Detailed Comparison:\n");

        var comparison = new[]
        {
            ("Cosmos DB", "Global NoSQL", "$$$", "Complex apps, gaming, IoT"),
            ("Blob Storage", "Object storage", "$$", "Media, backups, data lakes"),
            ("Table Storage", "Key-value NoSQL", "$", "Simple NoSQL, logs"),
            ("Queue Storage", "Message queue", "$", "Async processing, decoupling"),
            ("Azure Files", "SMB file shares", "$$", "Lift-and-shift, shared files")
        };

        foreach (var (service, type, cost, useCase) in comparison)
        {
            Console.WriteLine($"?? {service}");
            Console.WriteLine($"   Type: {type}");
            Console.WriteLine($"   Cost: {cost}");
            Console.WriteLine($"   Use Case: {useCase}\n");
        }
    }

    static void DisplayBestPractices()
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  STORAGE BEST PRACTICES");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? COSMOS DB:");
        Console.WriteLine("   ? Choose partition key based on query patterns");
        Console.WriteLine("   ? Use point reads (id + partition key) for 1 RU");
        Console.WriteLine("   ? Session consistency is default (99% of scenarios)");
        Console.WriteLine("   ? Use change feed for real-time sync");
        Console.WriteLine("   ? Index only necessary properties");
        Console.WriteLine("   ? Consider serverless for variable workloads\n");

        Console.WriteLine("?? BLOB STORAGE:");
        Console.WriteLine("   ? Use Cool tier for infrequent access (30+ days)");
        Console.WriteLine("   ? Use Archive tier for long-term storage (180+ days)");
        Console.WriteLine("   ? Enable soft delete for data protection");
        Console.WriteLine("   ? Use SAS tokens with minimal permissions");
        Console.WriteLine("   ? Implement lifecycle management policies");
        Console.WriteLine("   ? Use CDN for frequently accessed content\n");

        Console.WriteLine("?? TABLE STORAGE:");
        Console.WriteLine("   ? Design partition key for even distribution");
        Console.WriteLine("   ? Query within single partition when possible");
        Console.WriteLine("   ? Use batch operations for multiple entities");
        Console.WriteLine("   ? Keep entity size under 1 MB\n");

        Console.WriteLine("?? QUEUE STORAGE:");
        Console.WriteLine("   ? Set appropriate visibility timeout");
        Console.WriteLine("   ? Handle poison messages (move to dead-letter)");
        Console.WriteLine("   ? Keep messages small (max 64 KB)");
        Console.WriteLine("   ? Use Service Bus for advanced scenarios\n");
    }

    static void DisplayExamTips()
    {
        Console.Clear();
        Console.WriteLine("???????????????????????????????????????????????????????????????");
        Console.WriteLine("  AZ-204 EXAM TIPS - MODULE 2 (STORAGE)");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? HIGH FREQUENCY TOPICS:\n");

        Console.WriteLine("1. Cosmos DB Partitioning (?????)");
        Console.WriteLine("   • Know how to choose partition key");
        Console.WriteLine("   • Understand hot partitions");
        Console.WriteLine("   • Point reads vs queries\n");

        Console.WriteLine("2. Cosmos DB Consistency Levels (?????)");
        Console.WriteLine("   • Know all 5 levels");
        Console.WriteLine("   • Session is default");
        Console.WriteLine("   • Trade-offs: consistency vs latency vs cost\n");

        Console.WriteLine("3. Blob SAS Tokens (?????)");
        Console.WriteLine("   • Account SAS vs Service SAS");
        Console.WriteLine("   • Permissions (read, write, delete, list)");
        Console.WriteLine("   • Expiration times\n");

        Console.WriteLine("4. Blob Access Tiers (????)");
        Console.WriteLine("   • Hot: Frequent access, higher storage cost");
        Console.WriteLine("   • Cool: Infrequent (30+ days), lower storage cost");
        Console.WriteLine("   • Archive: Rare access (180+ days), lowest cost\n");

        Console.WriteLine("5. Request Units (RU) (????)");
        Console.WriteLine("   • Point read = 1 RU");
        Console.WriteLine("   • Cross-partition query = expensive");
        Console.WriteLine("   • Optimization techniques\n");

        Console.WriteLine("?? COMMON EXAM SCENARIOS:\n");

        Console.WriteLine("Scenario 1: 'Minimize cost for infrequently accessed data'");
        Console.WriteLine("   Answer: Blob Cool or Archive tier\n");

        Console.WriteLine("Scenario 2: 'Need ACID transactions in Cosmos DB'");
        Console.WriteLine("   Answer: Stored procedures (single partition)\n");

        Console.WriteLine("Scenario 3: 'Provide temporary access to blob'");
        Console.WriteLine("   Answer: Generate SAS token\n");

        Console.WriteLine("Scenario 4: 'Query consuming too many RUs'");
        Console.WriteLine("   Answer: Use partition key in query, limit results\n");

        Console.WriteLine("?? COMMON MISTAKES:");
        Console.WriteLine("   • Forgetting partition key in queries");
        Console.WriteLine("   • Using Strong consistency when not needed");
        Console.WriteLine("   • Not setting SAS token expiration");
        Console.WriteLine("   • Choosing wrong blob tier for access pattern\n");
    }
}
