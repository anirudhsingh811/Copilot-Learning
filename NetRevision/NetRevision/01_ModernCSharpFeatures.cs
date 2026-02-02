namespace NetRevision.CSharpFundamentals;

/// <summary>
/// POC: Modern C# Features (C# 10-14)
/// Demonstrates latest language features relevant for senior interviews
/// </summary>
public class ModernCSharpFeatures
{
    public static void RunDemo()
    {
        Console.WriteLine("=== Modern C# Features Demo ===\n");

        // 1. Record Types (C# 9+)
        RecordTypesDemo();

        // 2. Init-only Properties (C# 9)
        InitOnlyPropertiesDemo();

        // 3. Pattern Matching Enhancements
        PatternMatchingDemo();

        // 4. Global Usings & File-scoped Namespaces (C# 10)
        Console.WriteLine("? File-scoped namespaces & global usings in use\n");

        // 5. Required Members (C# 11)
        RequiredMembersDemo();

        // 6. Raw String Literals (C# 11)
        RawStringLiteralsDemo();

        // 7. Primary Constructors (C# 12)
        PrimaryConstructorsDemo();

        // 8. Collection Expressions (C# 12)
        CollectionExpressionsDemo();

        // 9. Ref readonly parameters (C# 12)
        RefReadonlyDemo();

        // 10. Nullable Reference Types
        NullableReferenceTypesDemo();
    }

    #region 1. Record Types
    private static void RecordTypesDemo()
    {
        Console.WriteLine("1. Record Types - Value-based Equality");

        var person1 = new Person("John", "Doe", 30);
        var person2 = new Person("John", "Doe", 30);
        var person3 = person1 with { Age = 31 }; // Non-destructive mutation

        Console.WriteLine($"person1 == person2: {person1 == person2}"); // True
        Console.WriteLine($"person3: {person3}");
        Console.WriteLine($"Deconstruction: {person1}");

        var (firstName, lastName, age) = person1;
        Console.WriteLine($"Deconstructed: {firstName} {lastName}, {age}\n");
    }

    public record Person(string FirstName, string LastName, int Age);
    
    // Record struct (C# 10) - Value type with record semantics
    public record struct Point(int X, int Y);
    #endregion

    #region 2. Init-only Properties
    private static void InitOnlyPropertiesDemo()
    {
        Console.WriteLine("2. Init-only Properties - Immutability");

        var employee = new Employee
        {
            Id = 1,
            Name = "Alice",
            Department = "Engineering"
        };

        // employee.Id = 2; // Compilation error - init-only
        Console.WriteLine($"Employee: {employee.Name} in {employee.Department}\n");
    }

    public class Employee
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public string? Department { get; init; }
    }
    #endregion

    #region 3. Pattern Matching
    private static void PatternMatchingDemo()
    {
        Console.WriteLine("3. Advanced Pattern Matching");

        object[] items = { 42, "Hello", null, new Person("Jane", "Smith", 25), 100 };

        foreach (var item in items)
        {
            var result = item switch
            {
                int n when n > 50 => $"Large number: {n}",
                int n => $"Small number: {n}",
                string { Length: > 0 } s => $"Non-empty string: {s}",
                Person { Age: >= 18 and < 65 } p => $"Working age person: {p.FirstName}",
                null => "Null value",
                _ => "Unknown type"
            };
            Console.WriteLine(result);
        }

        // List patterns (C# 11)
        int[] numbers = { 1, 2, 3, 4, 5 };
        var message = numbers switch
        {
            [1, 2, ..] => "Starts with 1, 2",
            [.., 5] => "Ends with 5",
            _ => "Other pattern"
        };
        Console.WriteLine($"List pattern: {message}\n");
    }
    #endregion

    #region 4. Required Members
    private static void RequiredMembersDemo()
    {
        Console.WriteLine("4. Required Members (C# 11)");

        var config = new AppConfig
        {
            ConnectionString = "Server=localhost;Database=test",
            ApiKey = "secret-key"
        };

        Console.WriteLine($"Config initialized with required members\n");
    }

    public class AppConfig
    {
        public required string ConnectionString { get; init; }
        public required string ApiKey { get; init; }
        public int Timeout { get; init; } = 30; // Optional with default
    }
    #endregion

    #region 5. Raw String Literals
    private static void RawStringLiteralsDemo()
    {
        Console.WriteLine("5. Raw String Literals (C# 11)");

        string json = """
            {
                "name": "John Doe",
                "age": 30,
                "address": {
                    "city": "New York"
                }
            }
            """;

        Console.WriteLine($"JSON without escaping:\n{json}\n");
    }
    #endregion

    #region 6. Primary Constructors
    private static void PrimaryConstructorsDemo()
    {
        Console.WriteLine("6. Primary Constructors (C# 12)");

        var service = new LoggingService("MyApp");
        service.Log("Application started");
        Console.WriteLine();
    }

    public class LoggingService(string applicationName)
    {
        private readonly string _appName = applicationName;

        public void Log(string message)
        {
            Console.WriteLine($"[{_appName}] {message}");
        }
    }
    #endregion

    #region 7. Collection Expressions
    private static void CollectionExpressionsDemo()
    {
        Console.WriteLine("7. Collection Expressions (C# 12)");

        // Simplified collection initialization
        int[] numbers = [1, 2, 3, 4, 5];
        List<string> names = ["Alice", "Bob", "Charlie"];

        // Spread operator
        int[] moreNumbers = [..numbers, 6, 7, 8];

        Console.WriteLine($"Numbers: {string.Join(", ", numbers)}");
        Console.WriteLine($"Names: {string.Join(", ", names)}");
        Console.WriteLine($"More numbers: {string.Join(", ", moreNumbers)}\n");
    }
    #endregion

    #region 8. Ref Readonly
    private static void RefReadonlyDemo()
    {
        Console.WriteLine("8. Ref Readonly Parameters (C# 12)");

        var largeStruct = new LargeStruct { Value = 42, Data = new int[1000] };
        ProcessLargeStruct(in largeStruct);
        Console.WriteLine($"Original value: {largeStruct.Value}\n");
    }

    private static void ProcessLargeStruct(ref readonly LargeStruct data)
    {
        // Can read but not modify - efficient for large structs
        Console.WriteLine($"Processing value: {data.Value}");
    }

    public struct LargeStruct
    {
        public int Value { get; set; }
        public int[] Data { get; set; }
    }
    #endregion

    #region 9. Nullable Reference Types
    private static void NullableReferenceTypesDemo()
    {
        Console.WriteLine("9. Nullable Reference Types");

        string? nullableString = GetNullableValue();
        string nonNullableString = GetNonNullableValue();

        // Null-forgiving operator
        Console.WriteLine($"Length: {nullableString?.Length ?? 0}");
        Console.WriteLine($"Non-nullable: {nonNullableString}\n");
    }

    private static string? GetNullableValue() => null;
    private static string GetNonNullableValue() => "Always has value";
    #endregion
}

/*
 * ==========================================
 * INTERVIEW QUESTIONS - Modern C# Features
 * ==========================================
 * 
 * BASIC:
 * 1. What's the difference between record and class?
 *    Answer: Records provide value-based equality, built-in ToString(), with-expressions,
 *    and are immutable by default. Classes have reference equality.
 * 
 * 2. Explain init-only properties vs readonly fields.
 *    Answer: Init-only can be set during object initialization. Readonly only in 
 *    constructor. Init provides more flexibility with object initializers.
 * 
 * 3. What are primary constructors and their benefits?
 *    Answer: Simplified syntax for constructor parameters directly in class declaration.
 *    Reduces boilerplate code.
 * 
 * INTERMEDIATE:
 * 4. How does pattern matching improve over traditional switch?
 *    Answer: Type patterns, property patterns, relational patterns, logical patterns,
 *    list patterns. More expressive and type-safe than traditional switch.
 * 
 * 5. Explain the performance benefit of ref readonly parameters.
 *    Answer: Passes large structs by reference avoiding copying, while ensuring immutability.
 *    Best for large value types.
 * 
 * 6. What are collection expressions and the spread operator?
 *    Answer: Unified syntax for creating collections [1,2,3]. Spread (..) allows
 *    combining collections efficiently.
 * 
 * ADVANCED:
 * 7. How do nullable reference types prevent null reference exceptions?
 *    Answer: Compiler warnings when potentially null values are dereferenced.
 *    Forces explicit null handling with ?, !, ??, ?.
 * 
 * 8. When would you use record struct vs record class?
 *    Answer: Record struct for small value types stored on stack. Record class for
 *    reference types with value semantics needing heap allocation.
 * 
 * 9. Explain the memory implications of with-expressions on records.
 *    Answer: Creates a new instance with shallow copy of members. For reference type
 *    members, only references are copied, not deep cloning.
 * 
 * 10. How do required members improve API design?
 *     Answer: Compile-time enforcement of mandatory properties in object initializers.
 *     Better than constructor parameters for many optional properties.
 * 
 * SCENARIO-BASED:
 * 11. You have a DTO with 20 properties, 5 required. What C# feature would you use?
 *     Answer: Required members + init-only properties. Cleaner than constructor with
 *     many parameters or separate validation.
 * 
 * 12. Design a configuration system using modern C# features.
 *     Answer: Records for immutable config, required members for mandatory settings,
 *     with-expressions for overrides, pattern matching for validation.
 * 
 * 13. How would you refactor legacy null checking code to use modern C# features?
 *     Answer: Enable nullable reference types, use null-conditional operators (?., ??),
 *     pattern matching with null checks, ArgumentNullException.ThrowIfNull().
 */
