using System.Diagnostics;

namespace NetRevision.CSharpFundamentals;

/// <summary>
/// POC: Memory Management & Performance Optimization
/// Critical knowledge for senior-level technical interviews
/// </summary>
public class MemoryAndPerformance
{
    public static void RunDemo()
    {
        Console.WriteLine("=== Memory Management & Performance Demo ===\n");

        // 1. Value Types vs Reference Types
        ValueVsReferenceTypesDemo();

        // 2. Stack vs Heap Allocation
        StackVsHeapDemo();

        // 3. Boxing and Unboxing
        BoxingUnboxingDemo();

        // 4. Span<T> and Memory<T>
        SpanMemoryDemo();

        // 5. String Optimization
        StringOptimizationDemo();

        // 6. Object Pooling
        ObjectPoolingDemo();

        // 7. Struct vs Class Performance
        StructVsClassPerformanceDemo();

        // 8. IDisposable and Using Statement
        DisposablePatternDemo();
    }

    #region 1. Value Types vs Reference Types
    private static void ValueVsReferenceTypesDemo()
    {
        Console.WriteLine("1. Value Types vs Reference Types\n");

        // Value type - stored on stack
        int valueType1 = 10;
        int valueType2 = valueType1;
        valueType2 = 20;
        Console.WriteLine($"Value Type - Original: {valueType1}, Copy: {valueType2}");

        // Reference type - stored on heap
        var refType1 = new PersonClass { Name = "John" };
        var refType2 = refType1;
        refType2.Name = "Jane";
        Console.WriteLine($"Reference Type - Original: {refType1.Name}, Copy: {refType2.Name}");

        // Struct (value type)
        var structType1 = new PersonStruct { Name = "Alice" };
        var structType2 = structType1;
        structType2.Name = "Bob";
        Console.WriteLine($"Struct - Original: {structType1.Name}, Copy: {structType2.Name}\n");
    }

    private class PersonClass { public string Name { get; set; } = string.Empty; }
    private struct PersonStruct { public string Name { get; set; } }
    #endregion

    #region 2. Stack vs Heap
    private static void StackVsHeapDemo()
    {
        Console.WriteLine("2. Stack vs Heap Allocation\n");

        // Stack allocation - fast, automatic cleanup
        int stackVar = 42; // Cleaned up when method exits
        
        // Heap allocation - managed by GC
        var heapVar = new List<int> { 1, 2, 3 }; // Garbage collected later

        // Stack allocation for span (no heap allocation)
        Span<int> stackSpan = stackalloc int[100];
        stackSpan[0] = 99;

        Console.WriteLine($"Stack var: {stackVar}");
        Console.WriteLine($"Heap var count: {heapVar.Count}");
        Console.WriteLine($"Stack span first element: {stackSpan[0]}\n");
    }
    #endregion

    #region 3. Boxing and Unboxing
    private static void BoxingUnboxingDemo()
    {
        Console.WriteLine("3. Boxing and Unboxing (Performance Impact)\n");

        var sw = Stopwatch.StartNew();

        // Boxing - value type to reference type (heap allocation)
        int value = 42;
        object boxed = value; // Boxing occurs
        Console.WriteLine($"Boxed: {boxed}");

        // Unboxing - reference type to value type
        int unboxed = (int)boxed; // Unboxing occurs
        Console.WriteLine($"Unboxed: {unboxed}");

        // Performance impact demonstration
        sw.Restart();
        for (int i = 0; i < 1_000_000; i++)
        {
            object obj = i; // Boxing - SLOW
        }
        Console.WriteLine($"Boxing 1M times: {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        for (int i = 0; i < 1_000_000; i++)
        {
            int val = i; // No boxing - FAST
        }
        Console.WriteLine($"No boxing 1M times: {sw.ElapsedMilliseconds}ms\n");
    }
    #endregion

    #region 4. Span<T> and Memory<T>
    private static void SpanMemoryDemo()
    {
        Console.WriteLine("4. Span<T> and Memory<T> - Modern Memory Management\n");

        // Traditional approach - allocates new array
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int[] slice = numbers[2..5]; // Allocates new array

        // Span<T> - no allocation, just a view
        Span<int> spanSlice = numbers.AsSpan()[2..5];
        spanSlice[0] = 99; // Modifies original array
        Console.WriteLine($"Original array after span modification: {numbers[2]}");

        // Performance comparison
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 1_000_000; i++)
        {
            _ = numbers[2..5]; // Array slicing - allocates
        }
        Console.WriteLine($"Array slicing: {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        for (int i = 0; i < 1_000_000; i++)
        {
            _ = numbers.AsSpan()[2..5]; // Span - no allocation
        }
        Console.WriteLine($"Span slicing: {sw.ElapsedMilliseconds}ms");

        // ReadOnlySpan for immutability
        ReadOnlySpan<int> readOnlySpan = numbers;
        Console.WriteLine($"ReadOnlySpan length: {readOnlySpan.Length}\n");
    }
    #endregion

    #region 5. String Optimization
    private static void StringOptimizationDemo()
    {
        Console.WriteLine("5. String Optimization\n");

        var sw = Stopwatch.StartNew();

        // Bad: String concatenation in loop (creates many objects)
        sw.Restart();
        string bad = "";
        for (int i = 0; i < 1000; i++)
        {
            bad += i.ToString(); // Creates new string each time
        }
        Console.WriteLine($"String concatenation: {sw.ElapsedMilliseconds}ms");

        // Good: StringBuilder
        sw.Restart();
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            sb.Append(i);
        }
        string good = sb.ToString();
        Console.WriteLine($"StringBuilder: {sw.ElapsedMilliseconds}ms");

        // Best for formatting: String interpolation
        string name = "John";
        int age = 30;
        string message = $"Name: {name}, Age: {age}";

        // String interning
        string str1 = "Hello";
        string str2 = "Hello";
        Console.WriteLine($"String interning (same reference): {ReferenceEquals(str1, str2)}");

        // Span<char> for string manipulation without allocation
        string original = "Hello World";
        ReadOnlySpan<char> span = original.AsSpan(0, 5);
        Console.WriteLine($"Span string slice: {span.ToString()}\n");
    }
    #endregion

    #region 6. Object Pooling
    private static void ObjectPoolingDemo()
    {
        Console.WriteLine("6. Object Pooling Pattern\n");

        var pool = new SimpleObjectPool<ExpensiveObject>(
            () => new ExpensiveObject(),
            maxSize: 10
        );

        var sw = Stopwatch.StartNew();

        // Without pooling
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            var obj = new ExpensiveObject();
            obj.DoWork();
            // GC will collect
        }
        Console.WriteLine($"Without pooling: {sw.ElapsedMilliseconds}ms");

        // With pooling
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            var obj = pool.Rent();
            obj.DoWork();
            pool.Return(obj);
        }
        Console.WriteLine($"With pooling: {sw.ElapsedMilliseconds}ms\n");
    }

    private class ExpensiveObject
    {
        private readonly byte[] _buffer = new byte[1024];
        public void DoWork() { /* Simulate work */ }
    }

    private class SimpleObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool = new();
        private readonly Func<T> _factory;
        private readonly int _maxSize;

        public SimpleObjectPool(Func<T> factory, int maxSize)
        {
            _factory = factory;
            _maxSize = maxSize;
        }

        public T Rent()
        {
            lock (_pool)
            {
                return _pool.Count > 0 ? _pool.Pop() : _factory();
            }
        }

        public void Return(T obj)
        {
            lock (_pool)
            {
                if (_pool.Count < _maxSize)
                    _pool.Push(obj);
            }
        }
    }
    #endregion

    #region 7. Struct vs Class Performance
    private static void StructVsClassPerformanceDemo()
    {
        Console.WriteLine("7. Struct vs Class Performance\n");

        var sw = Stopwatch.StartNew();

        // Class allocation (heap)
        sw.Restart();
        for (int i = 0; i < 1_000_000; i++)
        {
            var obj = new PointClass(i, i);
        }
        Console.WriteLine($"Class allocation: {sw.ElapsedMilliseconds}ms");

        // Struct allocation (stack)
        sw.Restart();
        for (int i = 0; i < 1_000_000; i++)
        {
            var obj = new PointStruct(i, i);
        }
        Console.WriteLine($"Struct allocation: {sw.ElapsedMilliseconds}ms\n");
    }

    private class PointClass
    {
        public int X { get; }
        public int Y { get; }
        public PointClass(int x, int y) { X = x; Y = y; }
    }

    private readonly struct PointStruct
    {
        public int X { get; }
        public int Y { get; }
        public PointStruct(int x, int y) { X = x; Y = y; }
    }
    #endregion

    #region 8. IDisposable and Using
    private static void DisposablePatternDemo()
    {
        Console.WriteLine("8. IDisposable Pattern - Resource Management\n");

        // Using statement - automatic disposal
        using (var resource = new ManagedResource())
        {
            resource.DoWork();
        } // Dispose called automatically

        // Using declaration (C# 8+)
        using var resource2 = new ManagedResource();
        resource2.DoWork();
        // Disposed at end of scope

        Console.WriteLine("Resources disposed\n");
    }

    private class ManagedResource : IDisposable
    {
        private bool _disposed;

        public void DoWork()
        {
            Console.WriteLine("Working with resource...");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    Console.WriteLine("Disposing managed resources");
                }
                // Dispose unmanaged resources
                _disposed = true;
            }
        }

        ~ManagedResource()
        {
            Dispose(false);
        }
    }
    #endregion
}

/*
 * =========================================================
 * INTERVIEW QUESTIONS - Memory Management & Performance
 * =========================================================
 * 
 * BASIC:
 * 1. Explain the difference between stack and heap memory.
 *    Answer: Stack: Fast, LIFO, automatic cleanup, limited size, stores value types and references.
 *    Heap: Slower, managed by GC, larger size, stores reference types (objects).
 * 
 * 2. What is boxing and unboxing? Why should we avoid it?
 *    Answer: Boxing converts value type to object (heap allocation). Unboxing is reverse.
 *    Causes performance issues due to heap allocations and GC pressure.
 * 
 * 3. When should you use struct vs class?
 *    Answer: Struct: Small, immutable types (<16 bytes), frequently allocated. 
 *    Class: Complex types, inheritance needed, mutable state, large objects.
 * 
 * INTERMEDIATE:
 * 4. Explain Span<T> and its benefits.
 *    Answer: Stack-only type providing view over contiguous memory. Zero allocation slicing,
 *    works with arrays, stack memory, unmanaged memory. Cannot be stored on heap.
 * 
 * 5. What's the difference between Memory<T> and Span<T>?
 *    Answer: Span<T> is stack-only, cannot be used in async methods or stored as field.
 *    Memory<T> can be stored on heap, used in async methods, but slightly slower.
 * 
 * 6. How does string interning work in .NET?
 *    Answer: Runtime maintains pool of unique string literals. Same literal reuses same
 *    memory reference. Only for compile-time literals, not runtime concatenation.
 * 
 * 7. Explain the IDisposable pattern and why GC.SuppressFinalize is used.
 *    Answer: IDisposable for deterministic cleanup of unmanaged resources. 
 *    GC.SuppressFinalize tells GC to skip finalizer queue, improving performance.
 * 
 * ADVANCED:
 * 8. What are the three generations in .NET garbage collection?
 *    Answer: Gen 0: Short-lived objects, collected frequently. Gen 1: Buffer between Gen 0/2.
 *    Gen 2: Long-lived objects, collected rarely. Large Object Heap for objects > 85KB.
 * 
 * 9. Explain object pooling and when to use it.
 *    Answer: Reuse expensive objects instead of creating new ones. Use for objects with
 *    high allocation cost (large memory, initialization overhead). ArrayPool<T>, ObjectPool<T>.
 * 
 * 10. What is the Large Object Heap (LOH) and how does it differ from regular heap?
 *     Answer: For objects >= 85KB. Not compacted by default (causes fragmentation).
 *     Collected only in Gen 2. Can enable compaction with GCSettings.LargeObjectHeapCompactionMode.
 * 
 * 11. How does stackalloc work and what are its limitations?
 *     Answer: Allocates memory on stack. Very fast, no GC. Limited by stack size (~1MB).
 *     Memory freed automatically when method exits. Use with Span<T>.
 * 
 * SCENARIO-BASED:
 * 12. You have a web API processing millions of small objects. Performance is poor. Diagnose.
 *     Answer: Check GC pressure (PerfView, dotMemory). Consider: struct for small DTOs,
 *     object pooling, Span<T> for buffers, reduce allocations in hot paths.
 * 
 * 13. Application has memory leak. How do you identify and fix?
 *     Answer: Use memory profiler (dotMemory, PerfView). Look for: event handlers not unsubscribed,
 *     static collections growing, large object retention, timers not disposed.
 * 
 * 14. Design a high-performance buffer management system.
 *     Answer: Use ArrayPool<byte>.Shared for pooling. Span<T> for slicing without allocation.
 *     Memory<T> for async scenarios. Consider custom pool for specific sizes.
 * 
 * 15. When would you use ref struct?
 *     Answer: For stack-only types like Span<T>. When you need guarantee of stack allocation
 *     and want to prevent heap allocation. Cannot be boxed, used in async, or stored as field.
 * 
 * LEADERSHIP/ARCHITECTURE:
 * 16. How would you approach performance optimization for a legacy .NET application?
 *     Answer: 1) Establish baseline (load tests, profiling). 2) Identify bottlenecks (CPU, memory, I/O).
 *     3) Quick wins (async/await, caching, connection pooling). 4) Deep optimization (allocations,
 *     algorithms). 5) Monitor and iterate.
 * 
 * 17. What metrics would you track for memory and performance in production?
 *     Answer: GC collections (Gen 0/1/2 count, pause time), memory usage (working set, private bytes),
 *     allocation rate, CPU usage, response times (p50, p95, p99), exception rate.
 */
