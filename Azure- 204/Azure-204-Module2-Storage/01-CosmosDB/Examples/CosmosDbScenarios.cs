using System.Text.Json;

namespace AZ204.Storage.CosmosDB.Examples;

/// <summary>
/// Interactive Cosmos DB scenarios for AZ-204 exam prep
/// </summary>
public static class CosmosDbScenarios
{
    /// <summary>
    /// Scenario 1: E-commerce product catalog with proper partitioning
    /// </summary>
    public static async Task<string> EcommercePartitioningDemo()
    {
        // Display the scenario header with emoji for visual identification
        Console.WriteLine("\n🎯 E-COMMERCE PARTITIONING STRATEGY");
        // Display a separator line for better readability
        Console.WriteLine("═══════════════════════════════════════════════\n");

        // Explain the chosen partition key strategy
        Console.WriteLine("📋 Partition Key: /category");
        // Provide the reasoning behind this partition key choice
        Console.WriteLine("   Why? Products are queried by category most often\n");

        // Create a sample array of product objects to demonstrate data structure
        // Is categoru a user defined column name in cosmos db or system defined?
        // Category is a user-defined property in Cosmos DB documents; it is not a system-defined column name. Then How does System treat it as partition key?
        // In Cosmos DB, the partition key is a user-defined property that you specify when creating a container. It is used by the system to distribute data across partitions for scalability and performance.
        // //In this example, "category" is chosen as the partition key because it aligns with common query patterns for the e-commerce product catalog.
        // How to define partition key in cosmos db? 
        // When creating a container in Cosmos DB, you define the partition key by specifying the path to the property that will serve as the partition key. This is done through the Azure portal, SDKs, or ARM templates.
        var products = new[]
        {
            // Sample product 1: Electronics category with high price and low stock
            new { id = "1", name = "Laptop", category = "electronics", price = 999.99, stock = 50 },
            // Sample product 2: Electronics category with low price and high stock
            new { id = "2", name = "Mouse", category = "electronics", price = 29.99, stock = 200 },
            // Sample product 3: Clothing category with low price and very high stock
            new { id = "3", name = "T-Shirt", category = "clothing", price = 19.99, stock = 500 },
            // Sample product 4: Clothing category with medium price and moderate stock
            new { id = "4", name = "Jeans", category = "clothing", price = 49.99, stock = 150 }
        };

        // Display header for efficient query examples
        Console.WriteLine("✅ Good Queries (Single Partition):");
        // Show example of a query that targets a single partition (efficient)
        Console.WriteLine("   SELECT * FROM c WHERE c.category = 'electronics'");
        // Display the low RU cost for single-partition queries
        Console.WriteLine("   RU Cost: ~3 RUs (efficient!)\n");

        // Simulate processing delay for better demo visualization
        await Task.Delay(500);

        // Display header for inefficient query examples
        Console.WriteLine("❌ Bad Queries (Cross-Partition):");
        // Show example of a query that scans across all partitions (inefficient)
        Console.WriteLine("   SELECT * FROM c WHERE c.price < 50");
        // Display the high RU cost for cross-partition queries
        Console.WriteLine("   RU Cost: ~20+ RUs (scans all partitions!)\n");

        // Simulate processing delay for better demo visualization
        await Task.Delay(500);

        // Display header for best practices section
        Console.WriteLine("💡 Best Practices:");
        // Best practice 1: Choose partition key based on how data is queried
        Console.WriteLine("   ✓ Choose partition key based on query patterns");
        // Best practice 2: Ensure data is distributed evenly across partitions
        Console.WriteLine("   ✓ Aim for even data distribution");
        // Best practice 3: Prevent scenarios where one partition gets too much traffic
        Console.WriteLine("   ✓ Avoid hot partitions");
        // Best practice 4: Use the most efficient read operation when possible
        Console.WriteLine("   ✓ Use point reads (id + partition key) when possible\n");

        // Serialize and return a JSON summary of the demo scenario
        return JsonSerializer.Serialize(new
        {
            // Scenario name for identification
            scenario = "E-commerce Partitioning",
            // The partition key strategy being demonstrated
            partitionKey = "/category",
            // Include the sample products used in the demo
            sampleProducts = products,
            // Object showing different query efficiency metrics
            efficiency = new
            {
                // Most efficient operation: reading by id and partition key
                pointRead = "1 RU",
                // Efficient: querying within a single partition
                singlePartitionQuery = "3-5 RUs",
                // Expensive: querying across multiple partitions
                crossPartitionQuery = "20+ RUs"
            }
        }, new JsonSerializerOptions { WriteIndented = true }); // Format JSON with indentation for readability
    }

    /// <summary>
    /// Scenario 2: Consistency levels explained
    /// </summary>
    public static async Task<string> ConsistencyLevelsDemo()// But how this consistence releated to partitioning? Do we have to mention partitioning here? or consistency levels only?
                                                            // Consistency levels are not directly related to partitioning, but they are an important aspect of Cosmos DB's data management. In this scenario, we will focus solely on explaining the different consistency levels available in Cosmos DB without delving into partitioning strategies.
                                                            //Is consistence a key in Cosmos or it's just user defined column name in db 
  //  Consistency levels in Cosmos DB are not user-defined column names;
  //  //they are predefined settings that determine how data consistency is managed across distributed databases.
  //  //Cosmos DB offers five different consistency levels: Strong, Bounded Staleness, Session, Consistent Prefix, and Eventual.
  //  //Each level provides a different balance between consistency, availability, and performance, allowing users to choose the one that best fits their application's requirements.

    {
        // Display the scenario header with emoji for visual identification
        Console.WriteLine("\n🔄 CONSISTENCY LEVELS DEMO");
        // Display a separator line for better readability
        Console.WriteLine("═══════════════════════════════════════════════\n");

        // Create a sample array of consistency level configurations to demonstrate different options
        var levels = new[]
        {
            // Strongest consistency: Guarantees reads always return the most recent write, highest cost and latency
            new { level = "Strong", latency = "Highest", consistency = "100%", cost = "Highest", useCase = "Financial transactions" },
            // Bounded staleness: Reads may lag behind writes by a specified time/operations, good balance for ordered data
            new { level = "Bounded Staleness", latency = "High", consistency = "99.9%", cost = "High", useCase = "Leaderboards, stock prices" },
            // Session consistency: Guarantees consistency within a single client session, default and most common choice
            new { level = "Session", latency = "Medium", consistency = "Within session", cost = "Medium", useCase = "User sessions (DEFAULT)" },
            // Consistent prefix: Guarantees reads never see out-of-order writes, but may lag behind
            new { level = "Consistent Prefix", latency = "Low", consistency = "Ordered reads", cost = "Low", useCase = "Social media feeds" },
            // Weakest consistency: Reads may return stale data, lowest cost and latency for non-critical scenarios
            new { level = "Eventual", latency = "Lowest", consistency = "Eventually", cost = "Lowest", useCase = "Product reviews, ratings" }
        };

        // Iterate through each consistency level to display its characteristics
        foreach (var level in levels)
        {
            // Simulate processing delay for better demo visualization
            await Task.Delay(400);
            // Display the consistency level name with emoji indicator
            Console.WriteLine($"📊 {level.level}:");
            // Show the latency characteristics for this consistency level
            Console.WriteLine($"   Latency: {level.latency}");
            // Show the consistency guarantee provided by this level
            Console.WriteLine($"   Consistency: {level.consistency}");
            // Show the cost implication (RU consumption) for this consistency level
            Console.WriteLine($"   Cost: {level.cost}");
            // Show a practical use case where this consistency level is appropriate
            Console.WriteLine($"   Use Case: {level.useCase}\n");
        }

        // Display header for exam preparation tips
        Console.WriteLine("💡 AZ-204 Exam Tip:");
        // Emphasize that Session is the default consistency level used in most scenarios
        Console.WriteLine("   SESSION is the default and most common (99% of scenarios)");
        // Highlight when Strong consistency is required for exam questions
        Console.WriteLine("   STRONG needed only for financial/critical data");
        // Explain when Eventual consistency is appropriate
        Console.WriteLine("   EVENTUAL for non-critical reads (reviews, comments)\n");

        // Serialize and return a JSON summary of the demo scenario
        return JsonSerializer.Serialize(new
        {
            // Scenario name for identification
            scenario = "Consistency Levels",
            // Indicate which consistency level is the default in Cosmos DB
            default_level = "Session",
            // Include all consistency levels with their characteristics
            levels = levels
        }, new JsonSerializerOptions { WriteIndented = true }); // Format JSON with indentation for readability
    }

    /// <summary>
    /// Scenario 3: Change feed for real-time sync
    /// </summary>
    public static async Task<string> ChangeFeedPatternDemo()
    {
        Console.WriteLine("\n?? CHANGE FEED PATTERN");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Use Case: Sync Cosmos DB ? Search Index");
        Console.WriteLine("   When: Product added/updated in Cosmos");
        Console.WriteLine("   Then: Update Azure Cognitive Search\n");

        await Task.Delay(500);

        Console.WriteLine("?? Simulating changes...\n");

        var changes = new[]
        {
            "Product added: Laptop Pro 15",
            "Price updated: $999 ? $899",
            "Stock changed: 50 ? 45",
            "Product deleted: Old Model X"
        };

        foreach (var change in changes)
        {
            await Task.Delay(600);
            Console.WriteLine($"   ?? Change detected: {change}");
            Console.WriteLine($"   ? Search index updated");
            Console.WriteLine();
        }

        Console.WriteLine("?? Change Feed Features:");
        Console.WriteLine("   ? Ordered, persistent record of changes");
        Console.WriteLine("   ? Asynchronous processing");
        Console.WriteLine("   ? Scalable across partitions");
        Console.WriteLine("   ? No deletes captured (use soft delete pattern)\n");

        Console.WriteLine("?? Common Patterns:");
        Console.WriteLine("   • Real-time sync (search, cache)");
        Console.WriteLine("   • Event sourcing");
        Console.WriteLine("   • Data archival");
        Console.WriteLine("   • Materialized views\n");

        return JsonSerializer.Serialize(new
        {
            scenario = "Change Feed Pattern",
            useCase = "Cosmos DB to Azure Cognitive Search sync",
            changes = changes,
            benefits = new[] { "Real-time sync", "Scalable", "Ordered", "Persistent" }
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Scenario 4: RU optimization techniques
    /// </summary>
    public static async Task<string> RUOptimizationDemo()
    {
        Console.WriteLine("\n?? REQUEST UNITS (RU) OPTIMIZATION");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Operation Costs:\n");

        var operations = new[]
        {
            ("Point Read (id + PK)", 1, "? Most efficient"),
            ("Single partition query", 3, "? Good"),
            ("Small cross-partition query", 20, "?? Moderate"),
            ("Large cross-partition query", 100, "? Expensive"),
            ("Write operation", 5, "? Reasonable"),
            ("Replace (without optimistic concurrency)", 5, "? Good"),
            ("Stored procedure (bulk ops)", 15, "? Batch efficiency")
        };

        foreach (var (operation, ru, note) in operations)
        {
            await Task.Delay(400);
            Console.WriteLine($"{operation}");
            Console.WriteLine($"   RU Cost: ~{ru} RUs");
            Console.WriteLine($"   {note}\n");
        }

        Console.WriteLine("?? Optimization Tips:");
        Console.WriteLine("   1. Always use point reads (id + partition key) when possible");
        Console.WriteLine("   2. Query within a single partition");
        Console.WriteLine("   3. Use SELECT to limit returned fields");
        Console.WriteLine("   4. Index only necessary properties");
        Console.WriteLine("   5. Use stored procedures for bulk operations");
        Console.WriteLine("   6. Consider serverless for variable workloads\n");

        Console.WriteLine("?? Exam Scenario:");
        Console.WriteLine("   Q: 'How to minimize RU consumption?'");
        Console.WriteLine("   A: Use point reads + partition key, limit results, index optimization\n");

        return JsonSerializer.Serialize(new
        {
            scenario = "RU Optimization",
            operations = operations.Select(o => new { name = o.Item1, ru = o.Item2, note = o.Item3 }),
            bestPractice = "Point reads with id + partition key"
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Scenario 5: Multi-region setup for global apps
    /// </summary>
    public static async Task<string> GlobalDistributionDemo()
    {
        Console.WriteLine("\n?? GLOBAL DISTRIBUTION");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Multi-Region Configuration:\n");

        var regions = new[]
        {
            ("East US", "Primary", "Read + Write"),
            ("West Europe", "Secondary", "Read"),
            ("Southeast Asia", "Secondary", "Read")
        };

        foreach (var (region, type, access) in regions)
        {
            await Task.Delay(400);
            Console.WriteLine($"   ?? {region}");
            Console.WriteLine($"      Type: {type}");
            Console.WriteLine($"      Access: {access}\n");
        }

        Console.WriteLine("?? Automatic Failover:");
        Console.WriteLine("   Scenario: East US region fails");
        await Task.Delay(600);
        Console.WriteLine("   ? Failover to West Europe (< 1 minute)");
        Console.WriteLine("   ? Writes now go to West Europe");
        Console.WriteLine("   ? No application code changes needed\n");

        Console.WriteLine("?? Benefits:");
        Console.WriteLine("   ? Low latency for global users");
        Console.WriteLine("   ? 99.999% availability SLA (5 nines!)");
        Console.WriteLine("   ? Automatic failover");
        Console.WriteLine("   ? Multi-region writes available\n");

        Console.WriteLine("?? Cost Consideration:");
        Console.WriteLine("   Each region doubles your RU cost");
        Console.WriteLine("   Example: 1000 RU/s × 3 regions = 3000 RU/s billed\n");

        return JsonSerializer.Serialize(new
        {
            scenario = "Global Distribution",
            regions = regions.Select(r => new { name = r.Item1, type = r.Item2, access = r.Item3 }),
            availability = "99.999%",
            failoverTime = "< 1 minute"
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Scenario 6: Stored procedures for transactions
    /// </summary>
    public static async Task<string> StoredProcedureDemo()
    {
        Console.WriteLine("\n?? STORED PROCEDURES (TRANSACTIONS)");
        Console.WriteLine("???????????????????????????????????????????????????????????????\n");

        Console.WriteLine("?? Scenario: Transfer money between accounts\n");

        Console.WriteLine("?? Transactional Batch:");
        Console.WriteLine("   1. Debit $100 from Account A");
        await Task.Delay(400);
        Console.WriteLine("   2. Credit $100 to Account B");
        await Task.Delay(400);
        Console.WriteLine("   3. Log transaction");
        await Task.Delay(400);
        Console.WriteLine("\n   ? All succeed or all fail (ACID)\n");

        Console.WriteLine("?? Why Stored Procedures?");
        Console.WriteLine("   ? Atomic operations");
        Console.WriteLine("   ? Reduced network round-trips");
        Console.WriteLine("   ? Lower RU consumption");
        Console.WriteLine("   ? Server-side logic\n");

        Console.WriteLine("?? Limitations:");
        Console.WriteLine("   • Scoped to single partition key");
        Console.WriteLine("   • JavaScript only");
        Console.WriteLine("   • 5-second execution limit\n");

        Console.WriteLine("?? Exam Tip:");
        Console.WriteLine("   For cross-partition transactions ? Use Durable Functions");
        Console.WriteLine("   For single-partition ACID ? Use stored procedures\n");

        return JsonSerializer.Serialize(new
        {
            scenario = "Stored Procedures",
            useCase = "Money transfer (transactional)",
            benefits = new[] { "ACID", "Reduced RU", "Atomic operations" },
            limitation = "Single partition only"
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}
