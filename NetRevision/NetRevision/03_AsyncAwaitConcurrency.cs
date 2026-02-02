using System.Diagnostics;

namespace NetRevision.CSharpFundamentals;

/// <summary>
/// POC: Async/Await and Concurrency Patterns
/// Essential for modern .NET development and senior interviews
/// </summary>
public class AsyncAwaitConcurrency
{
    public static async Task RunDemo()
    {
        Console.WriteLine("=== Async/Await and Concurrency Demo ===\n");

        // 1. Basic Async/Await
        await BasicAsyncAwaitDemo();

        // 2. ConfigureAwait
        await ConfigureAwaitDemo();

        // 3. Task Parallel Library
        await TaskParallelDemo();

        // 4. Parallel Programming
        ParallelProgrammingDemo();

        // 5. Thread Safety
        await ThreadSafetyDemo();

        // 6. Concurrent Collections
        ConcurrentCollectionsDemo();

        // 7. Async Enumerable
        await AsyncEnumerableDemo();

        // 8. ValueTask
        await ValueTaskDemo();
    }

    #region 1. Basic Async/Await
    private static async Task BasicAsyncAwaitDemo()
    {
        Console.WriteLine("1. Basic Async/Await Pattern\n");

        // Synchronous - blocks thread
        var sw = Stopwatch.StartNew();
        FetchDataSync();
        FetchDataSync();
        Console.WriteLine($"Synchronous execution: {sw.ElapsedMilliseconds}ms");

        // Asynchronous - non-blocking
        sw.Restart();
        await FetchDataAsync();
        await FetchDataAsync();
        Console.WriteLine($"Sequential async execution: {sw.ElapsedMilliseconds}ms");

        // Parallel async execution
        sw.Restart();
        var task1 = FetchDataAsync();
        var task2 = FetchDataAsync();
        await Task.WhenAll(task1, task2);
        Console.WriteLine($"Parallel async execution: {sw.ElapsedMilliseconds}ms\n");
    }

    private static void FetchDataSync()
    {
        Thread.Sleep(100); // Simulate I/O
    }

    private static async Task<string> FetchDataAsync()
    {
        await Task.Delay(100); // Simulate async I/O
        return "Data";
    }
    #endregion

    #region 2. ConfigureAwait
    private static async Task ConfigureAwaitDemo()
    {
        Console.WriteLine("2. ConfigureAwait Pattern\n");

        // ConfigureAwait(false) - Don't capture context (better performance for library code)
        await DoWorkWithoutContextAsync();

        // ConfigureAwait(true) or default - Capture synchronization context (needed for UI)
        await DoWorkWithContextAsync();

        Console.WriteLine("ConfigureAwait demo completed\n");
    }

    private static async Task DoWorkWithoutContextAsync()
    {
        await Task.Delay(50).ConfigureAwait(false);
        Console.WriteLine($"Thread after ConfigureAwait(false): {Thread.CurrentThread.ManagedThreadId}");
    }

    private static async Task DoWorkWithContextAsync()
    {
        await Task.Delay(50); // ConfigureAwait(true) by default
        Console.WriteLine($"Thread after ConfigureAwait(true): {Thread.CurrentThread.ManagedThreadId}");
    }
    #endregion

    #region 3. Task Parallel Library
    private static async Task TaskParallelDemo()
    {
        Console.WriteLine("3. Task Parallel Library (TPL)\n");

        // Task.Run - offload work to thread pool
        var result = await Task.Run(() =>
        {
            Thread.Sleep(100);
            return 42;
        });
        Console.WriteLine($"Task.Run result: {result}");

        // Task.WhenAll - wait for all tasks
        var tasks = Enumerable.Range(1, 5)
            .Select(i => ProcessItemAsync(i))
            .ToArray();
        var results = await Task.WhenAll(tasks);
        Console.WriteLine($"WhenAll results: {string.Join(", ", results)}");

        // Task.WhenAny - wait for first completion
        var firstCompleted = await Task.WhenAny(
            Task.Delay(100),
            Task.Delay(200),
            Task.Delay(300)
        );
        Console.WriteLine($"First task completed");

        // CancellationToken
        await CancellationDemo();

        Console.WriteLine();
    }

    private static async Task<int> ProcessItemAsync(int item)
    {
        await Task.Delay(50);
        return item * 2;
    }

    private static async Task CancellationDemo()
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(200));

        try
        {
            await LongRunningOperationAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled");
        }
    }

    private static async Task LongRunningOperationAsync(CancellationToken ct)
    {
        for (int i = 0; i < 10; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(100, ct);
        }
    }
    #endregion

    #region 4. Parallel Programming
    private static void ParallelProgrammingDemo()
    {
        Console.WriteLine("4. Parallel Programming\n");

        var numbers = Enumerable.Range(1, 1000).ToArray();

        // Parallel.For
        var sw = Stopwatch.StartNew();
        Parallel.For(0, numbers.Length, i =>
        {
            numbers[i] = numbers[i] * 2;
        });
        Console.WriteLine($"Parallel.For: {sw.ElapsedMilliseconds}ms");

        // Parallel.ForEach
        sw.Restart();
        Parallel.ForEach(numbers, number =>
        {
            // Process each item in parallel
            _ = number * 2;
        });
        Console.WriteLine($"Parallel.ForEach: {sw.ElapsedMilliseconds}ms");

        // PLINQ
        sw.Restart();
        var plinqResults = numbers
            .AsParallel()
            .Where(n => n % 2 == 0)
            .Select(n => n * 2)
            .ToArray();
        Console.WriteLine($"PLINQ processing: {sw.ElapsedMilliseconds}ms, Results: {plinqResults.Length}\n");
    }
    #endregion

    #region 5. Thread Safety
    private static async Task ThreadSafetyDemo()
    {
        Console.WriteLine("5. Thread Safety Patterns\n");

        // 1. Lock statement
        await LockDemo();

        // 2. SemaphoreSlim for async
        await SemaphoreDemo();

        // 3. Interlocked for atomic operations
        InterlockedDemo();

        Console.WriteLine();
    }

    private static readonly object _lockObject = new();
    private static int _counter = 0;

    private static async Task LockDemo()
    {
        _counter = 0;
        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                lock (_lockObject)
                {
                    _counter++;
                }
            }
        }));

        await Task.WhenAll(tasks);
        Console.WriteLine($"Lock demo - Counter: {_counter} (expected: 10000)");
    }

    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    private static async Task SemaphoreDemo()
    {
        _counter = 0;
        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(async () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                await _semaphore.WaitAsync();
                try
                {
                    _counter++;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }));

        await Task.WhenAll(tasks);
        Console.WriteLine($"SemaphoreSlim demo - Counter: {_counter} (expected: 10000)");
    }

    private static void InterlockedDemo()
    {
        _counter = 0;
        var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                Interlocked.Increment(ref _counter);
            }
        }));

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"Interlocked demo - Counter: {_counter} (expected: 10000)");
    }
    #endregion

    #region 6. Concurrent Collections
    private static void ConcurrentCollectionsDemo()
    {
        Console.WriteLine("6. Concurrent Collections\n");

        // ConcurrentBag - unordered collection
        var bag = new System.Collections.Concurrent.ConcurrentBag<int>();
        Parallel.For(0, 1000, i => bag.Add(i));
        Console.WriteLine($"ConcurrentBag count: {bag.Count}");

        // ConcurrentQueue - FIFO
        var queue = new System.Collections.Concurrent.ConcurrentQueue<int>();
        Parallel.For(0, 100, i => queue.Enqueue(i));
        queue.TryDequeue(out var item);
        Console.WriteLine($"ConcurrentQueue dequeued: {item}");

        // ConcurrentDictionary - thread-safe dictionary
        var dict = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        Parallel.For(0, 100, i =>
        {
            dict.AddOrUpdate($"key{i}", 1, (key, oldValue) => oldValue + 1);
        });
        Console.WriteLine($"ConcurrentDictionary count: {dict.Count}\n");
    }
    #endregion

    #region 7. Async Enumerable
    private static async Task AsyncEnumerableDemo()
    {
        Console.WriteLine("7. Async Enumerable (IAsyncEnumerable<T>)\n");

        await foreach (var item in GenerateNumbersAsync())
        {
            Console.WriteLine($"Received: {item}");
        }

        Console.WriteLine();
    }

    private static async IAsyncEnumerable<int> GenerateNumbersAsync()
    {
        for (int i = 1; i <= 5; i++)
        {
            await Task.Delay(100); // Simulate async operation
            yield return i;
        }
    }
    #endregion

    #region 8. ValueTask
    private static async Task ValueTaskDemo()
    {
        Console.WriteLine("8. ValueTask vs Task\n");

        // ValueTask - reduces allocations when result is often synchronous
        var result1 = await GetValueAsync(true);
        Console.WriteLine($"ValueTask (cached): {result1}");

        var result2 = await GetValueAsync(false);
        Console.WriteLine($"ValueTask (async): {result2}");

        // Performance comparison
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 100000; i++)
        {
            await GetValueAsync(true); // Often synchronous - ValueTask shines here
        }
        Console.WriteLine($"ValueTask performance: {sw.ElapsedMilliseconds}ms\n");
    }

    private static ValueTask<int> GetValueAsync(bool cached)
    {
        if (cached)
        {
            // Synchronous path - no allocation with ValueTask
            return new ValueTask<int>(42);
        }

        // Asynchronous path
        return new ValueTask<int>(Task.Run(async () =>
        {
            await Task.Delay(1);
            return 42;
        }));
    }
    #endregion
}

/*
 * ======================================================
 * INTERVIEW QUESTIONS - Async/Await and Concurrency
 * ======================================================
 * 
 * BASIC:
 * 1. What's the difference between async and parallel programming?
 *    Answer: Async is about non-blocking I/O operations (doesn't use extra threads for waiting).
 *    Parallel is about CPU-bound work using multiple threads simultaneously.
 * 
 * 2. Explain async/await keywords.
 *    Answer: async marks method can contain await. await yields control back to caller while
 *    operation completes, allowing thread to do other work. Returns Task or Task<T>.
 * 
 * 3. What is ConfigureAwait(false) and when to use it?
 *    Answer: Prevents capturing synchronization context. Use in library code for better performance.
 *    Don't use in UI code where you need to return to UI thread.
 * 
 * INTERMEDIATE:
 * 4. Difference between Task.Run and Task.Factory.StartNew?
 *    Answer: Task.Run is simpler, uses default scheduler, returns Task<T>.
 *    Task.Factory.StartNew is more configurable but returns Task<Task<T>> for async delegates.
 * 
 * 5. When would you use ValueTask instead of Task?
 *    Answer: When result is often available synchronously (cached, fast path). Reduces allocations.
 *    Don't use for multiple awaits or when storing as field.
 * 
 * 6. Explain the difference between Task.WhenAll and Task.WaitAll.
 *    Answer: WhenAll is async, returns Task, doesn't block thread. WaitAll is synchronous,
 *    blocks calling thread. WhenAll preferred in async code.
 * 
 * 7. How does CancellationToken work?
 *    Answer: Cooperative cancellation mechanism. Token passed to async operations.
 *    Operation checks token.IsCancellationRequested or calls ThrowIfCancellationRequested.
 * 
 * 8. What's the difference between lock and SemaphoreSlim?
 *    Answer: lock cannot be awaited (not for async code). SemaphoreSlim.WaitAsync() is async-friendly.
 *    SemaphoreSlim also allows limiting concurrent access count.
 * 
 * ADVANCED:
 * 9. Explain synchronization context and how async/await uses it.
 *    Answer: Captures execution context (UI thread, ASP.NET context). After await, continuation
 *    runs on captured context by default. ConfigureAwait(false) skips capture.
 * 
 * 10. What are the dangers of async void methods?
 *     Answer: Can't await them, exceptions crash app, no way to track completion. Only use for
 *     event handlers. Always return Task for testability and error handling.
 * 
 * 11. Explain deadlock scenario with async/await and how to prevent it.
 *     Answer: Calling .Result or .Wait() on async code blocks thread while holding context.
 *     Async code tries to return to same context - deadlock. Prevent: use await, ConfigureAwait(false),
 *     never block on async code.
 * 
 * 12. How does TaskScheduler work and when would you create a custom one?
 *     Answer: Controls how Tasks are scheduled on threads. Default uses ThreadPool.
 *     Custom scheduler for: UI thread, limiting concurrency, priority scheduling.
 * 
 * 13. Explain the difference between Parallel.ForEach and PLINQ.
 *     Answer: Parallel.ForEach for imperative loops with side effects. PLINQ for declarative
 *     queries, returns results. PLINQ has overhead, better for CPU-intensive operations.
 * 
 * SCENARIO-BASED:
 * 14. API endpoint has 1000 concurrent requests, each needs to call 3 external services. Design solution.
 *     Answer: Use async/await (don't block threads). Task.WhenAll for parallel external calls.
 *     SemaphoreSlim to limit concurrent external calls. Implement timeouts and cancellation.
 *     Consider circuit breaker pattern.
 * 
 * 15. How do you handle fire-and-forget scenarios safely?
 *     Answer: Avoid if possible. If needed: use IHostedService or BackgroundService, log exceptions,
 *     use CancellationToken for graceful shutdown, consider messaging queue for reliability.
 * 
 * 16. Application has CPU-bound work mixed with I/O operations. How to optimize?
 *     Answer: Use async/await for I/O (doesn't consume threads). Task.Run for CPU-bound work
 *     to avoid blocking. Consider Task.Yield() to improve responsiveness. Profile to identify bottlenecks.
 * 
 * 17. Explain how you would implement retry logic with exponential backoff asynchronously.
 *     Answer: Use Polly library or custom implementation with Task.Delay. Exponentially increase
 *     delay: delay = baseDelay * Math.Pow(2, attempt). Add jitter to prevent thundering herd.
 *     Pass CancellationToken for graceful cancellation.
 * 
 * LEADERSHIP/ARCHITECTURE:
 * 18. How do you ensure your team writes correct async code?
 *     Answer: Code reviews checking for blocking calls (.Result, .Wait()), async all the way down,
 *     proper exception handling, CancellationToken usage. Use analyzers (AsyncFixer).
 *     Establish patterns and guidelines.
 * 
 * 19. What considerations for async programming in high-load production systems?
 *     Answer: ThreadPool starvation monitoring, async timeouts, circuit breakers, bulkhead isolation,
 *     proper logging (correlation IDs), load testing, chaos engineering, monitoring async method
 *     completion times and exception rates.
 */
