using NetRevision.CSharpFundamentals;
using NetRevision.DotNetCore;
using NetRevision.AspNetCore;
using NetRevision.DataAccess;
using NetRevision.DesignPatterns;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  .NET Tech Stack Interview Revision - POC Collection      ║");
Console.WriteLine("║  For Engineering Managers with 14+ Years Experience       ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

while (true)
{
    Console.WriteLine("\n=== MAIN MENU ===");
    Console.WriteLine("\n📚 C# & Language Fundamentals:");
    Console.WriteLine("1. Modern C# Features (C# 10-14)");
    Console.WriteLine("2. Memory Management & Performance");
    Console.WriteLine("3. Async/Await & Concurrency");
    
    Console.WriteLine("\n🌐 Web & Data:");
    Console.WriteLine("4. Dependency Injection & IoC");
    Console.WriteLine("5. ASP.NET Core Web API Patterns");
    Console.WriteLine("6. Entity Framework Core");
    
    Console.WriteLine("\n🏗️ Architecture & Design:");
    Console.WriteLine("7. Design Patterns & Architecture");
    
    Console.WriteLine("\n⚡ Actions:");
    Console.WriteLine("8. Run All Demos");
    Console.WriteLine("9. Show Quick Reference Guide");
    Console.WriteLine("0. Exit");
    
    Console.Write("\n👉 Select option (0-9): ");

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                ModernCSharpFeatures.RunDemo();
                break;
            case "2":
                MemoryAndPerformance.RunDemo();
                break;
            case "3":
                await AsyncAwaitConcurrency.RunDemo();
                break;
            case "4":
                DependencyInjectionPatterns.RunDemo();
                break;
            case "5":
                AspNetCorePatterns.RunDemo();
                break;
            case "6":
                EntityFrameworkPatterns.RunDemo();
                break;
            case "7":
                DesignPatternsArchitecture.RunDemo();
                break;
            case "8":
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("RUNNING ALL DEMOS...");
                Console.WriteLine(new string('=', 60) + "\n");
                
                ModernCSharpFeatures.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                MemoryAndPerformance.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                await AsyncAwaitConcurrency.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                DependencyInjectionPatterns.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                AspNetCorePatterns.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                EntityFrameworkPatterns.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                DesignPatternsArchitecture.RunDemo();
                Console.WriteLine(new string('=', 60));
                
                Console.WriteLine("\n✅ ALL DEMOS COMPLETED!");
                break;
            case "9":
                ShowQuickReferenceMenu();
                break;
            case "0":
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("✓ Thank you for using .NET Interview Revision!");
                Console.WriteLine("✓ Review interview questions in each source file");
                Console.WriteLine("✓ Check INTERVIEW_QUESTIONS.md for 200+ questions");
                Console.WriteLine("✓ Follow STUDY_PLAN.md for 8-week revision");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine("\n💪 Happy Interviewing! Good luck!");
                return;
            default:
                Console.WriteLine("\n❌ Invalid choice. Please select 0-9.");
                continue;
        }

        if (choice != "9")
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.WriteLine("✓ Demo completed!");
            Console.WriteLine("📝 Review interview questions at the end of the source file");
            Console.WriteLine("📚 Check documentation:");
            Console.WriteLine("   • INTERVIEW_QUESTIONS.md - 200 comprehensive questions");
            Console.WriteLine("   • STUDY_PLAN.md - 8-week revision schedule");
            Console.WriteLine("   • QUICK_REFERENCE.md - Last-minute cheat sheet");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error: {ex.Message}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}

static void ShowQuickReferenceMenu()
{
    Console.Clear();
    Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║           📖 QUICK REFERENCE GUIDE                         ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");
    
    Console.WriteLine("📁 Available Documentation:");
    Console.WriteLine();
    Console.WriteLine("1. QUICK_REFERENCE.md");
    Console.WriteLine("   → Code snippets, patterns, cheat sheet");
    Console.WriteLine("   → Essential concepts for quick revision");
    Console.WriteLine();
    Console.WriteLine("2. INTERVIEW_QUESTIONS.md");
    Console.WriteLine("   → 200 comprehensive interview questions");
    Console.WriteLine("   → Categorized by topic and difficulty");
    Console.WriteLine("   → Includes leadership & system design");
    Console.WriteLine();
    Console.WriteLine("3. STUDY_PLAN.md");
    Console.WriteLine("   → 8-week structured revision plan");
    Console.WriteLine("   → Daily routines and hands-on exercises");
    Console.WriteLine("   → Progress tracking checklist");
    Console.WriteLine();
    Console.WriteLine("4. README.md");
    Console.WriteLine("   → Project overview & learning phases");
    Console.WriteLine("   → Complete tech stack coverage");
    Console.WriteLine();
    Console.WriteLine("5. GETTING_STARTED.md");
    Console.WriteLine("   → Setup guide and usage instructions");
    Console.WriteLine("   → How to use this revision project");
    Console.WriteLine();
    
    Console.WriteLine(new string('-', 60));
    Console.WriteLine("💡 TIP: Open these files in your editor for detailed content");
    Console.WriteLine(new string('-', 60));
    Console.WriteLine();
    Console.WriteLine("🔍 Quick Topics Overview:");
    Console.WriteLine();
    Console.WriteLine("C# & Fundamentals:");
    Console.WriteLine("  • Records, Pattern Matching, Primary Constructors");
    Console.WriteLine("  • Span<T>, Memory<T>, Object Pooling");
    Console.WriteLine("  • Async/Await, Task Parallel Library, ValueTask");
    Console.WriteLine();
    Console.WriteLine("Web & Data:");
    Console.WriteLine("  • Dependency Injection, Service Lifetimes");
    Console.WriteLine("  • ASP.NET Core Middleware, Minimal APIs");
    Console.WriteLine("  • Entity Framework Core, Query Optimization");
    Console.WriteLine();
    Console.WriteLine("Architecture & Design:");
    Console.WriteLine("  • SOLID Principles");
    Console.WriteLine("  • Design Patterns (Creational, Structural, Behavioral)");
    Console.WriteLine("  • CQRS, Clean Architecture, DDD");
    Console.WriteLine();
    Console.WriteLine("Leadership & System Design:");
    Console.WriteLine("  • Microservices vs Monolith");
    Console.WriteLine("  • Scalability & Performance");
    Console.WriteLine("  • Team Management & Technical Decisions");
    Console.WriteLine();
    
    Console.WriteLine(new string('=', 60));
    Console.WriteLine("Press any key to return to main menu...");
    Console.ReadKey();
    Console.Clear();
}
