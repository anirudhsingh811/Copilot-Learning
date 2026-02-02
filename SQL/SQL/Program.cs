using SQLPreparation;
using System;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║        SQL PREPARATION GUIDE - 2 Week Study Plan              ║");
Console.WriteLine("║        For Professionals with 13+ Years Experience            ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

Console.WriteLine("📚 WEEK 1: Core Concepts & Advanced Queries");
Console.WriteLine("   Day 1-2:  DDL Operations");
Console.WriteLine("   Day 3-4:  DML Operations & JOINs");
Console.WriteLine("   Day 5-6:  Window Functions & CTEs");
Console.WriteLine("   Day 7:    Aggregations & Set Operations\n");

Console.WriteLine("🚀 WEEK 2: Performance & Advanced Topics");
Console.WriteLine("   Day 8-9:  Indexing & Performance Optimization");
Console.WriteLine("   Day 10-11: Transactions & Concurrency");
Console.WriteLine("   Day 12-13: Stored Procedures & Advanced Features");
Console.WriteLine("   Day 14:   Interview Questions & Scenarios\n");

Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

bool exit = false;

while (!exit)
{
    Console.WriteLine("\n📋 SELECT A DEMO TO RUN:");
    Console.WriteLine("─────────────────────────────────────────────────────────");
    Console.WriteLine("1.  DDL Operations (CREATE, ALTER, DROP, Constraints)");
    Console.WriteLine("2.  DML Operations (INSERT, UPDATE, DELETE, MERGE)");
    Console.WriteLine("3.  JOINs & Subqueries (All JOIN types, Correlated queries)");
    Console.WriteLine("4.  Window Functions (ROW_NUMBER, RANK, LEAD, LAG)");
    Console.WriteLine("5.  CTEs & Recursive Queries (Hierarchical data)");
    Console.WriteLine("6.  Performance Optimization (Indexes, Execution Plans)");
    Console.WriteLine("7.  Transactions & Concurrency (ACID, Isolation, Locks)");
    Console.WriteLine("8.  Interview Questions (Real-world scenarios)");
    Console.WriteLine("9.  🌟 RUN ALL DEMOS (Complete walkthrough)");
    Console.WriteLine("0.  Exit");
    Console.WriteLine("─────────────────────────────────────────────────────────");
    Console.Write("Enter your choice: ");

    string choice = Console.ReadLine() ?? "";

    Console.WriteLine("\n");

    switch (choice)
    {
        case "1":
            DDL_Operations.RunAllDemos();
            break;
        case "2":
            DML_Operations.RunAllDemos();
            break;
        case "3":
            Joins_Subqueries.RunAllDemos();
            break;
        case "4":
            Window_Functions.RunAllDemos();
            break;
        case "5":
            CTEs_Recursive.RunAllDemos();
            break;
        case "6":
            Performance_Optimization.RunAllDemos();
            break;
        case "7":
            Transactions_Concurrency.RunAllDemos();
            break;
        case "8":
            Interview_Questions.RunAllDemos();
            break;
        case "9":
            Console.WriteLine("🚀 RUNNING COMPLETE SQL PREPARATION GUIDE...\n");
            RunAllDemos();
            break;
        case "0":
            exit = true;
            Console.WriteLine("✅ Thank you for using SQL Preparation Guide!");
            Console.WriteLine("💡 Pro Tip: Practice these concepts daily for best results.");
            Console.WriteLine("🎯 Focus on understanding, not memorization.");
            break;
        default:
            Console.WriteLine("❌ Invalid choice. Please select 0-9.");
            break;
    }

    if (!exit && choice != "0")
    {
        Console.WriteLine("\n" + new string('═', 65));
        Console.WriteLine("Press any key to return to main menu...");
        Console.ReadKey();
        Console.Clear();
    }
}

static void RunAllDemos()
{
    try
    {
        Console.WriteLine("\n📚 WEEK 1 - Day 1-2: DDL Operations");
        Console.WriteLine(new string('─', 65));
        DDL_Operations.RunAllDemos();

        Console.WriteLine("\n\n📚 WEEK 1 - Day 3-4: DML Operations");
        Console.WriteLine(new string('─', 65));
        DML_Operations.RunAllDemos();

        Console.WriteLine("\n\n📚 WEEK 1 - Day 3-4: JOINs & Subqueries");
        Console.WriteLine(new string('─', 65));
        Joins_Subqueries.RunAllDemos();

        Console.WriteLine("\n\n📚 WEEK 1 - Day 5-6: Window Functions");
        Console.WriteLine(new string('─', 65));
        Window_Functions.RunAllDemos();

        Console.WriteLine("\n\n📚 WEEK 1 - Day 5-6: CTEs & Recursive Queries");
        Console.WriteLine(new string('─', 65));
        CTEs_Recursive.RunAllDemos();

        Console.WriteLine("\n\n🚀 WEEK 2 - Day 8-9: Performance Optimization");
        Console.WriteLine(new string('─', 65));
        Performance_Optimization.RunAllDemos();

        Console.WriteLine("\n\n🚀 WEEK 2 - Day 10-11: Transactions & Concurrency");
        Console.WriteLine(new string('─', 65));
        Transactions_Concurrency.RunAllDemos();

        Console.WriteLine("\n\n🎯 WEEK 2 - Day 14: Interview Questions");
        Console.WriteLine(new string('─', 65));
        Interview_Questions.RunAllDemos();

        Console.WriteLine("\n\n✅ CONGRATULATIONS! You've completed all demos!");
        Console.WriteLine("🎓 Keep practicing these concepts for mastery.");
        Console.WriteLine("💪 You're ready for SQL interviews!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error running demos: {ex.Message}");
        Console.WriteLine("💡 Make sure SQL Server is running and connection string is configured.");
    }
}
