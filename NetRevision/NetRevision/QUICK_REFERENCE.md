# ?? QUICK REFERENCE CHEAT SHEET
## Essential .NET Concepts for Engineering Manager Interviews

---

## C# MODERN FEATURES QUICK REFERENCE

### Records (C# 9+)
```csharp
// Immutable by default, value-based equality
public record Person(string Name, int Age);
var p1 = new Person("John", 30);
var p2 = p1 with { Age = 31 }; // Non-destructive mutation
```

### Pattern Matching (C# 8-12)
```csharp
// Type patterns, property patterns, relational patterns
var result = obj switch
{
    int n when n > 50 => "Large",
    string { Length: > 0 } s => "Non-empty",
    Person { Age: >= 18 and < 65 } => "Working age",
    [1, 2, ..] => "Starts with 1, 2", // List pattern (C# 11)
    _ => "Other"
};
```

### Required Members (C# 11)
```csharp
public class Config
{
    public required string ConnectionString { get; init; }
    public int Timeout { get; init; } = 30;
}
```

### Primary Constructors (C# 12)
```csharp
public class Logger(string appName)
{
    public void Log(string msg) => Console.WriteLine($"[{appName}] {msg}");
}
```

---

## MEMORY & PERFORMANCE

### Span<T> - Zero Allocation Slicing
```csharp
int[] arr = { 1, 2, 3, 4, 5 };
Span<int> span = arr.AsSpan()[2..4]; // No allocation
span[0] = 99; // Modifies original array
```

### Object Pooling
```csharp
var pool = ArrayPool<byte>.Shared;
byte[] buffer = pool.Rent(1024);
try { /* use buffer */ }
finally { pool.Return(buffer); }
```

### IDisposable Pattern
```csharp
public class Resource : IDisposable
{
    private bool _disposed;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Dispose managed resources
        }
        _disposed = true;
    }
    
    ~Resource() => Dispose(false);
}
```

### String Optimization
```csharp
// ? Bad - creates many string objects
string bad = "";
for (int i = 0; i < 1000; i++) bad += i;

// ? Good - single allocation
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++) sb.Append(i);
```

---

## ASYNC/AWAIT PATTERNS

### Basic Pattern
```csharp
public async Task<string> GetDataAsync()
{
    await Task.Delay(100); // Non-blocking wait
    return "Data";
}
```

### ConfigureAwait
```csharp
// Library code - don't capture context
await SomeMethodAsync().ConfigureAwait(false);

// UI/ASP.NET - capture context (default)
await SomeMethodAsync();
```

### Parallel Execution
```csharp
// Sequential (slow)
await Task1Async();
await Task2Async();

// Parallel (fast)
var t1 = Task1Async();
var t2 = Task2Async();
await Task.WhenAll(t1, t2);
```

### Cancellation
```csharp
public async Task DoWorkAsync(CancellationToken ct)
{
    while (!ct.IsCancellationRequested)
    {
        await Task.Delay(100, ct);
        // Work...
    }
}
```

### ValueTask - Optimize Common Case
```csharp
public ValueTask<int> GetCachedAsync(int id)
{
    if (_cache.TryGetValue(id, out var value))
        return new ValueTask<int>(value); // No allocation
    
    return new ValueTask<int>(FetchFromDbAsync(id));
}
```

---

## DEPENDENCY INJECTION

### Service Lifetimes
```csharp
// Singleton - one instance for app lifetime
services.AddSingleton<ICache, MemoryCache>();

// Scoped - one instance per request/scope
services.AddScoped<DbContext>();

// Transient - new instance every time
services.AddTransient<IEmailService, EmailService>();
```

### Multiple Implementations
```csharp
services.AddTransient<INotification, EmailNotification>();
services.AddTransient<INotification, SmsNotification>();

// Resolve all
public class NotificationManager
{
    public NotificationManager(IEnumerable<INotification> notifications) { }
}
```

### Factory Pattern
```csharp
services.AddTransient<IFactory<IService>>(sp => type => 
    (IService)sp.GetRequiredService(type));
```

---

## ASP.NET CORE ESSENTIALS

### Minimal API (Clean)
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/products/{id}", async (int id, IProductService svc) =>
{
    var product = await svc.GetByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.Run();
```

### Custom Middleware
```csharp
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    
    public LoggingMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Before
        await _next(context);
        // After
    }
}

// Register: app.UseMiddleware<LoggingMiddleware>();
```

### Action Filter
```csharp
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(context.ModelState);
    }
}
```

### JWT Authentication
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
```

---

## ENTITY FRAMEWORK CORE

### DbContext Configuration
```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Sku).IsUnique();
        });
    }
}
```

### Query Optimization
```csharp
// ? N+1 Problem
var orders = await context.Orders.ToListAsync();
foreach (var order in orders)
    Console.WriteLine(order.Customer.Name); // N queries

// ? Eager Loading
var orders = await context.Orders
    .Include(o => o.Customer)
    .ToListAsync();

// ? Projection (best for DTOs)
var orders = await context.Orders
    .Select(o => new OrderDto 
    { 
        OrderId = o.Id, 
        CustomerName = o.Customer.Name 
    })
    .ToListAsync();
```

### AsNoTracking for Read-Only
```csharp
var products = await context.Products
    .AsNoTracking() // Better performance for read-only
    .ToListAsync();
```

---

## DESIGN PATTERNS

### Repository Pattern
```csharp
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Unit of Work
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Order> Orders { get; }
    Task<int> SaveChangesAsync();
}
```

### CQRS with MediatR
```csharp
// Command
public record CreateOrderCommand(int CustomerId, List<OrderItem> Items) 
    : IRequest<int>;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // Create order logic
        return orderId;
    }
}

// Usage
var orderId = await mediator.Send(new CreateOrderCommand(123, items));
```

### Strategy Pattern
```csharp
public interface IPaymentStrategy
{
    Task<bool> ProcessPayment(decimal amount);
}

public class PaymentProcessor
{
    private readonly IPaymentStrategy _strategy;
    
    public PaymentProcessor(IPaymentStrategy strategy) => _strategy = strategy;
    
    public Task<bool> ProcessAsync(decimal amount) => 
        _strategy.ProcessPayment(amount);
}
```

---

## TESTING PATTERNS

### Unit Test (xUnit + Moq)
```csharp
public class OrderServiceTests
{
    [Fact]
    public async Task ProcessOrder_ValidOrder_ReturnsSuccess()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(true);
        var service = new OrderService(mockRepo.Object);
        
        // Act
        var result = await service.ProcessOrderAsync(new Order());
        
        // Assert
        Assert.True(result);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}
```

### Integration Test
```csharp
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();
    }
}
```

---

## SECURITY BEST PRACTICES

### Password Hashing
```csharp
// Use BCrypt, Argon2, or PBKDF2
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
bool isValid = BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
```

### SQL Injection Prevention
```csharp
// ? Vulnerable
var query = $"SELECT * FROM Users WHERE Username = '{username}'";

// ? Parameterized
var query = "SELECT * FROM Users WHERE Username = @Username";
cmd.Parameters.AddWithValue("@Username", username);

// ? EF Core (already parameterized)
var user = await context.Users
    .FirstOrDefaultAsync(u => u.Username == username);
```

### CORS Configuration
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://example.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

---

## PERFORMANCE OPTIMIZATION CHECKLIST

### API Performance
- [ ] Use async/await for I/O operations
- [ ] Implement caching (in-memory, distributed)
- [ ] Use pagination for large datasets
- [ ] Optimize database queries (indexes, projections)
- [ ] Implement compression (gzip, brotli)
- [ ] Use CDN for static content
- [ ] Implement rate limiting

### Database Performance
- [ ] Add proper indexes
- [ ] Use query optimization (EXPLAIN ANALYZE)
- [ ] Implement connection pooling
- [ ] Use read replicas for scaling
- [ ] Denormalize where appropriate
- [ ] Use caching layers (Redis)
- [ ] Batch operations when possible

### Memory Optimization
- [ ] Use Span<T> for string/array manipulation
- [ ] Implement object pooling for expensive objects
- [ ] Dispose IDisposable objects properly
- [ ] Avoid boxing in hot paths
- [ ] Use struct for small, immutable types
- [ ] Profile with dotMemory/PerfView

---

## COMMON INTERVIEW TOPICS

### Explain in 2 Minutes:
1. **Async/Await**: Non-blocking I/O. Frees thread while waiting. State machine behind scenes.
2. **SOLID**: Five principles for maintainable OOP design (SRP, OCP, LSP, ISP, DIP).
3. **DI**: Inject dependencies instead of creating them. Improves testability and flexibility.
4. **EF Core**: ORM for .NET. Code-first or database-first. LINQ queries to SQL.
5. **Microservices**: Independently deployable services. Own databases. Communicate via APIs/messages.

### Whiteboard Design (15 minutes):
- High-traffic e-commerce platform
- Real-time notification system
- Video streaming service
- Payment processing system
- Distributed logging system

### Coding Challenges (30 minutes):
- Implement async retry with exponential backoff
- Design thread-safe cache with expiration
- Build rate limiter for API
- Create generic repository pattern
- Implement circuit breaker pattern

---

## RED FLAGS TO AVOID

### In Code:
- ? `async void` (except event handlers)
- ? Blocking on async (.Result, .Wait())
- ? Not disposing IDisposable
- ? String concatenation in loops
- ? Catching general exceptions without rethrowing

### In Architecture:
- ? Tight coupling between layers
- ? God classes/services
- ? Ignoring SOLID principles
- ? No error handling strategy
- ? Hardcoded configuration

### In Answers:
- ? "I don't know" without reasoning
- ? Over-engineering simple problems
- ? Not asking clarifying questions
- ? Not considering trade-offs
- ? Not admitting knowledge gaps

---

## QUICK WINS IN INTERVIEWS

### Technical:
? Write clean, readable code  
? Use meaningful variable names  
? Handle edge cases  
? Think about performance  
? Consider security implications

### Communication:
? Think out loud  
? Ask clarifying questions  
? Explain trade-offs  
? Admit when you don't know  
? Show eagerness to learn

### Leadership (EM specific):
? Share real experiences  
? Discuss team challenges  
? Show decision-making process  
? Mention metrics and outcomes  
? Balance technical and business needs

---

## LAST-MINUTE PREP (Day Before)

### Review These:
1. Your resume - know every project cold
2. Company's tech stack - research what they use
3. Your "greatest challenges" stories (STAR format)
4. System design templates
5. Recent .NET releases and features

### Practice:
- Explain SOLID with examples (10 min)
- Design a system on paper (15 min)
- Code a common algorithm (15 min)
- Answer "Tell me about yourself" (3 min)

### Prepare Questions:
- About team structure and dynamics
- About tech stack and architecture
- About deployment and DevOps practices
- About code review process
- About growth opportunities

---

**Remember**: Interviews assess problem-solving, communication, and culture fit—not just memorization. Be yourself, think critically, and enjoy the conversation!

**You've got this! ????**
