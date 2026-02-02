namespace NetRevision.DesignPatterns;

/// <summary>
/// POC: Design Patterns and Architecture
/// Common patterns for enterprise application development
/// </summary>
public class DesignPatternsArchitecture
{
    public static void RunDemo()
    {
        Console.WriteLine("=== Design Patterns & Architecture Demo ===\n");

        // 1. SOLID Principles
        SolidPrinciplesDemo();

        // 2. Creational Patterns
        CreationalPatternsDemo();

        // 3. Structural Patterns
        StructuralPatternsDemo();

        // 4. Behavioral Patterns
        BehavioralPatternsDemo();

        // 5. CQRS Pattern
        CqrsPatternDemo();

        // 6. Clean Architecture
        CleanArchitectureDemo();
    }

    #region 1. SOLID Principles
    private static void SolidPrinciplesDemo()
    {
        Console.WriteLine("1. SOLID Principles\n");

        Console.WriteLine("=== S - Single Responsibility Principle ===");
        Console.WriteLine("A class should have only one reason to change.\n");
        Console.WriteLine("? BAD:");
        Console.WriteLine(@"
public class User
{
    public void Save() { /* database logic */ }
    public void SendEmail() { /* email logic */ }
    public void ValidateData() { /* validation logic */ }
}
");
        Console.WriteLine("? GOOD:");
        Console.WriteLine(@"
public class User { /* properties only */ }
public class UserRepository { public void Save(User user) { } }
public class EmailService { public void SendEmail(string to) { } }
public class UserValidator { public bool Validate(User user) { } }
");

        Console.WriteLine("\n=== O - Open/Closed Principle ===");
        Console.WriteLine("Open for extension, closed for modification.\n");
        Console.WriteLine("? BAD:");
        Console.WriteLine(@"
public class PaymentProcessor
{
    public void ProcessPayment(string type, decimal amount)
    {
        if (type == ""CreditCard"") { /* logic */ }
        else if (type == ""PayPal"") { /* logic */ }
        // Adding new payment needs modification
    }
}
");
        Console.WriteLine("? GOOD:");
        Console.WriteLine(@"
public interface IPaymentStrategy { void Process(decimal amount); }
public class CreditCardPayment : IPaymentStrategy { }
public class PayPalPayment : IPaymentStrategy { }
// Add new payment without modifying existing code
");

        Console.WriteLine("\n=== L - Liskov Substitution Principle ===");
        Console.WriteLine("Subtypes must be substitutable for base types.\n");
        Console.WriteLine("? BAD:");
        Console.WriteLine(@"
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
}
public class Square : Rectangle
{
    public override int Width 
    { 
        set { base.Width = base.Height = value; } // Violates LSP
    }
}
");
        Console.WriteLine("? GOOD:");
        Console.WriteLine(@"
public interface IShape { int CalculateArea(); }
public class Rectangle : IShape { }
public class Square : IShape { }
");

        Console.WriteLine("\n=== I - Interface Segregation Principle ===");
        Console.WriteLine("Many specific interfaces better than one general.\n");
        Console.WriteLine("? BAD:");
        Console.WriteLine(@"
public interface IWorker
{
    void Work();
    void Eat(); // Not all workers eat (robots)
    void Sleep(); // Not all workers sleep
}
");
        Console.WriteLine("? GOOD:");
        Console.WriteLine(@"
public interface IWorkable { void Work(); }
public interface IFeedable { void Eat(); }
public interface ISleepable { void Sleep(); }
");

        Console.WriteLine("\n=== D - Dependency Inversion Principle ===");
        Console.WriteLine("Depend on abstractions, not concretions.\n");
        Console.WriteLine("? BAD:");
        Console.WriteLine(@"
public class OrderService
{
    private SqlServerRepository _repo = new(); // Tight coupling
}
");
        Console.WriteLine("? GOOD:");
        Console.WriteLine(@"
public class OrderService
{
    private readonly IRepository _repo;
    public OrderService(IRepository repo) { _repo = repo; }
}
");
        Console.WriteLine();
    }
    #endregion

    #region 2. Creational Patterns
    private static void CreationalPatternsDemo()
    {
        Console.WriteLine("2. Creational Design Patterns\n");

        Console.WriteLine("=== Singleton Pattern ===");
        Console.WriteLine(@"
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = 
        new Lazy<Logger>(() => new Logger());

    private Logger() { }

    public static Logger Instance => _instance.Value;

    public void Log(string message) { }
}
");

        Console.WriteLine("\n=== Factory Pattern ===");
        Console.WriteLine(@"
public interface INotification { void Send(string message); }

public class NotificationFactory
{
    public INotification Create(string type) => type switch
    {
        ""Email"" => new EmailNotification(),
        ""SMS"" => new SmsNotification(),
        ""Push"" => new PushNotification(),
        _ => throw new ArgumentException(""Invalid type"")
    };
}
");

        Console.WriteLine("\n=== Abstract Factory Pattern ===");
        Console.WriteLine(@"
public interface IUIFactory
{
    IButton CreateButton();
    ICheckbox CreateCheckbox();
}

public class WindowsFactory : IUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ICheckbox CreateCheckbox() => new WindowsCheckbox();
}

public class MacFactory : IUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ICheckbox CreateCheckbox() => new MacCheckbox();
}
");

        Console.WriteLine("\n=== Builder Pattern ===");
        Console.WriteLine(@"
public class QueryBuilder
{
    private string _table = string.Empty;
    private List<string> _columns = new();
    private string _where = string.Empty;

    public QueryBuilder Select(params string[] columns)
    {
        _columns.AddRange(columns);
        return this;
    }

    public QueryBuilder From(string table)
    {
        _table = table;
        return this;
    }

    public QueryBuilder Where(string condition)
    {
        _where = condition;
        return this;
    }

    public string Build()
    {
        return $""SELECT {string.Join("", "", _columns)} FROM {_table} WHERE {_where}"";
    }
}

// Usage
var query = new QueryBuilder()
    .Select(""Id"", ""Name"")
    .From(""Products"")
    .Where(""Price > 100"")
    .Build();
");
        Console.WriteLine();
    }
    #endregion

    #region 3. Structural Patterns
    private static void StructuralPatternsDemo()
    {
        Console.WriteLine("3. Structural Design Patterns\n");

        Console.WriteLine("=== Adapter Pattern ===");
        Console.WriteLine(@"
// Legacy interface
public class LegacyPaymentService
{
    public void MakePayment(string amount) { }
}

// New interface
public interface IPaymentProcessor
{
    Task ProcessPaymentAsync(decimal amount);
}

// Adapter
public class PaymentAdapter : IPaymentProcessor
{
    private readonly LegacyPaymentService _legacy;

    public PaymentAdapter(LegacyPaymentService legacy)
    {
        _legacy = legacy;
    }

    public async Task ProcessPaymentAsync(decimal amount)
    {
        await Task.Run(() => _legacy.MakePayment(amount.ToString()));
    }
}
");

        Console.WriteLine("\n=== Decorator Pattern ===");
        Console.WriteLine(@"
public interface IDataService
{
    Task<string> GetDataAsync();
}

public class BaseDataService : IDataService
{
    public async Task<string> GetDataAsync()
    {
        return await Task.FromResult(""Data"");
    }
}

public class CachedDataService : IDataService
{
    private readonly IDataService _inner;
    private readonly ICache _cache;

    public CachedDataService(IDataService inner, ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<string> GetDataAsync()
    {
        var cached = _cache.Get<string>(""data"");
        if (cached != null) return cached;

        var data = await _inner.GetDataAsync();
        _cache.Set(""data"", data);
        return data;
    }
}

public class LoggedDataService : IDataService
{
    private readonly IDataService _inner;
    private readonly ILogger _logger;

    public LoggedDataService(IDataService inner, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<string> GetDataAsync()
    {
        _logger.Log(""Getting data"");
        var data = await _inner.GetDataAsync();
        _logger.Log(""Data retrieved"");
        return data;
    }
}

// Usage: new LoggedDataService(new CachedDataService(new BaseDataService()))
");

        Console.WriteLine("\n=== Facade Pattern ===");
        Console.WriteLine(@"
public class OrderFacade
{
    private readonly IInventoryService _inventory;
    private readonly IPaymentService _payment;
    private readonly IShippingService _shipping;
    private readonly INotificationService _notification;

    public OrderFacade(/* inject dependencies */) { }

    public async Task<bool> PlaceOrderAsync(Order order)
    {
        // Simplifies complex subsystem interactions
        if (!await _inventory.CheckAvailabilityAsync(order.Items))
            return false;

        if (!await _payment.ProcessAsync(order.Total))
            return false;

        await _shipping.CreateShipmentAsync(order);
        await _notification.SendConfirmationAsync(order);

        return true;
    }
}
");
        Console.WriteLine();
    }
    #endregion

    #region 4. Behavioral Patterns
    private static void BehavioralPatternsDemo()
    {
        Console.WriteLine("4. Behavioral Design Patterns\n");

        Console.WriteLine("=== Strategy Pattern ===");
        Console.WriteLine(@"
public interface IShippingStrategy
{
    decimal CalculateCost(decimal weight, string destination);
}

public class StandardShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return weight * 0.5m;
    }
}

public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return weight * 1.5m;
    }
}

public class ShippingCalculator
{
    private readonly IShippingStrategy _strategy;

    public ShippingCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    public decimal Calculate(decimal weight, string destination)
    {
        return _strategy.CalculateCost(weight, destination);
    }
}
");

        Console.WriteLine("\n=== Observer Pattern ===");
        Console.WriteLine(@"
public interface IObserver
{
    void Update(string message);
}

public class EmailObserver : IObserver
{
    public void Update(string message)
    {
        Console.WriteLine($""Email sent: {message}"");
    }
}

public class Subject
{
    private readonly List<IObserver> _observers = new();

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void Notify(string message)
    {
        foreach (var observer in _observers)
        {
            observer.Update(message);
        }
    }
}

// Modern approach: Use events or IObservable<T>
");

        Console.WriteLine("\n=== Command Pattern ===");
        Console.WriteLine(@"
public interface ICommand
{
    Task ExecuteAsync();
    Task UndoAsync();
}

public class CreateOrderCommand : ICommand
{
    private readonly Order _order;
    private readonly IOrderRepository _repository;

    public CreateOrderCommand(Order order, IOrderRepository repository)
    {
        _order = order;
        _repository = repository;
    }

    public async Task ExecuteAsync()
    {
        await _repository.AddAsync(_order);
    }

    public async Task UndoAsync()
    {
        await _repository.DeleteAsync(_order.Id);
    }
}

public class CommandInvoker
{
    private readonly Stack<ICommand> _history = new();

    public async Task ExecuteAsync(ICommand command)
    {
        await command.ExecuteAsync();
        _history.Push(command);
    }

    public async Task UndoAsync()
    {
        if (_history.Count > 0)
        {
            var command = _history.Pop();
            await command.UndoAsync();
        }
    }
}
");

        Console.WriteLine("\n=== Chain of Responsibility ===");
        Console.WriteLine(@"
public abstract class ValidationHandler
{
    protected ValidationHandler? _next;

    public void SetNext(ValidationHandler next)
    {
        _next = next;
    }

    public abstract Task<bool> HandleAsync(Order order);
}

public class StockValidator : ValidationHandler
{
    public override async Task<bool> HandleAsync(Order order)
    {
        // Check stock
        if (/* stock available */)
            return _next?.HandleAsync(order) ?? await Task.FromResult(true);
        return false;
    }
}

public class PaymentValidator : ValidationHandler
{
    public override async Task<bool> HandleAsync(Order order)
    {
        // Validate payment
        if (/* payment valid */)
            return _next?.HandleAsync(order) ?? await Task.FromResult(true);
        return false;
    }
}

// Usage
var stockValidator = new StockValidator();
var paymentValidator = new PaymentValidator();
stockValidator.SetNext(paymentValidator);
await stockValidator.HandleAsync(order);
");
        Console.WriteLine();
    }
    #endregion

    #region 5. CQRS Pattern
    private static void CqrsPatternDemo()
    {
        Console.WriteLine("5. CQRS (Command Query Responsibility Segregation)\n");

        Console.WriteLine("Separates read and write operations.\n");

        Console.WriteLine("=== Commands (Write) ===");
        Console.WriteLine(@"
public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        await _repository.AddAsync(product);
        return product.Id;
    }
}
");

        Console.WriteLine("\n=== Queries (Read) ===");
        Console.WriteLine(@"
public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IReadDbContext _readDb;

    public GetProductByIdHandler(IReadDbContext readDb)
    {
        _readDb = readDb;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        return await _readDb.Products
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .FirstOrDefaultAsync(ct);
    }
}
");

        Console.WriteLine("\n=== Usage with MediatR ===");
        Console.WriteLine(@"
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet(""{id}"")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        return product != null ? Ok(product) : NotFound();
    }
}
");
        Console.WriteLine();
    }
    #endregion

    #region 6. Clean Architecture
    private static void CleanArchitectureDemo()
    {
        Console.WriteLine("6. Clean Architecture (Onion Architecture)\n");

        Console.WriteLine("Layer Structure (Dependency Rule: Inner layers don't depend on outer):\n");

        Console.WriteLine("???????????????????????????????????????????????");
        Console.WriteLine("?  Presentation Layer (API, UI)              ? ? Controllers, ViewModels");
        Console.WriteLine("?  ????????????????????????????????????????? ?");
        Console.WriteLine("?  ?  Application Layer                    ? ? ? Use Cases, DTOs");
        Console.WriteLine("?  ?  ??????????????????????????????????? ? ?");
        Console.WriteLine("?  ?  ?  Domain Layer                   ? ? ? ? Entities, Rules");
        Console.WriteLine("?  ?  ?  (Core Business Logic)          ? ? ?");
        Console.WriteLine("?  ?  ??????????????????????????????????? ? ?");
        Console.WriteLine("?  ????????????????????????????????????????? ?");
        Console.WriteLine("?  Infrastructure Layer                      ? ? DB, External APIs");
        Console.WriteLine("???????????????????????????????????????????????\n");

        Console.WriteLine("=== Domain Layer (Core) ===");
        Console.WriteLine(@"
public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new DomainException(""Price must be positive"");
        
        Price = newPrice;
    }
}

public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task AddAsync(Product product);
}
");

        Console.WriteLine("\n=== Application Layer ===");
        Console.WriteLine(@"
public class CreateProductUseCase
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductUseCase(IProductRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> ExecuteAsync(CreateProductRequest request)
    {
        var product = new Product(request.Name, request.Price);
        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product.Id;
    }
}
");

        Console.WriteLine("\n=== Infrastructure Layer ===");
        Console.WriteLine(@"
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }
}
");

        Console.WriteLine("\n=== Presentation Layer ===");
        Console.WriteLine(@"
[ApiController]
[Route(""api/[controller]"")]
public class ProductsController : ControllerBase
{
    private readonly CreateProductUseCase _createProduct;

    public ProductsController(CreateProductUseCase createProduct)
    {
        _createProduct = createProduct;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateProductRequest request)
    {
        var id = await _createProduct.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }
}
");

        Console.WriteLine("\nBenefits:");
        Console.WriteLine("? Testable (core has no dependencies)");
        Console.WriteLine("? Maintainable (clear separation)");
        Console.WriteLine("? Flexible (easy to swap infrastructure)");
        Console.WriteLine("? Independent (business logic isolated)\n");
    }
    #endregion
}

/*
 * =========================================================
 * INTERVIEW QUESTIONS - Design Patterns & Architecture
 * =========================================================
 * 
 * BASIC:
 * 1. Explain SOLID principles with examples.
 *    Answer: SRP (one reason to change), OCP (extend not modify), LSP (substitutable subtypes),
 *    ISP (specific interfaces), DIP (depend on abstractions). See code examples above.
 * 
 * 2. What is Dependency Injection and how does it relate to DI principle?
 *    Answer: DI pattern implements DIP. Inject dependencies instead of creating them.
 *    Inverts control, improves testability, reduces coupling.
 * 
 * 3. Difference between Factory and Abstract Factory patterns?
 *    Answer: Factory creates one type of object. Abstract Factory creates families of related
 *    objects (e.g., Windows UI components vs Mac UI components).
 * 
 * 4. When would you use Singleton pattern?
 *    Answer: When exactly one instance needed (configuration, logging, caching).
 *    CAUTION: Can be anti-pattern, prefer DI with singleton lifetime.
 * 
 * 5. Explain Strategy pattern with real-world example.
 *    Answer: Define family of algorithms, encapsulate each, make interchangeable.
 *    Example: Payment methods (credit card, PayPal, crypto). Shipping methods (standard, express).
 * 
 * INTERMEDIATE:
 * 6. What is CQRS and when should you use it?
 *    Answer: Separate read (query) and write (command) models. Use when: different optimization
 *    needs, complex domain, event sourcing, different teams for read/write.
 * 
 * 7. Explain Repository pattern. Should you use it with EF Core?
 *    Answer: Abstracts data access. Debatable with EF Core (already unit of work).
 *    USE: Complex queries, abstraction, testing. DON'T USE: Simple CRUD, over-abstraction.
 * 
 * 8. Difference between Adapter and Facade patterns?
 *    Answer: Adapter: Makes incompatible interfaces compatible (wraps single class).
 *    Facade: Simplifies complex subsystem (wraps multiple classes).
 * 
 * 9. What is Decorator pattern and how is it used in .NET?
 *    Answer: Adds behavior to objects dynamically. Examples: ASP.NET middleware,
 *    logging/caching wrappers, Stream decorators (BufferedStream, GZipStream).
 * 
 * 10. Explain Observer pattern. How is it implemented in C#?
 *     Answer: One-to-many dependency, notify observers of state changes.
 *     C#: events, IObservable<T>/IObserver<T>, event aggregator pattern.
 * 
 * ADVANCED:
 * 11. Explain Clean Architecture and its benefits.
 *     Answer: Onion architecture with dependency rule (inner layers independent).
 *     Layers: Domain (entities), Application (use cases), Infrastructure (DB, external),
 *     Presentation (UI, API). Testable, maintainable, technology-independent core.
 * 
 * 12. What is Domain-Driven Design (DDD)?
 *     Answer: Focus on domain model and business logic. Concepts: Entities, Value Objects,
 *     Aggregates, Repositories, Domain Events, Bounded Contexts, Ubiquitous Language.
 * 
 * 13. Explain Event Sourcing and its trade-offs.
 *     Answer: Store events instead of current state. Rebuild state by replaying events.
 *     PROS: Audit trail, time travel, event-driven. CONS: Complexity, eventual consistency,
 *     event versioning, query complexity. Use with CQRS.
 * 
 * 14. What is Saga pattern for distributed transactions?
 *     Answer: Sequence of local transactions with compensating transactions for rollback.
 *     Two types: Choreography (events), Orchestration (coordinator). Use in microservices.
 * 
 * 15. Explain Specification pattern and when to use it.
 *     Answer: Encapsulates business rules as objects. Compose with AND/OR/NOT.
 *     Benefits: Reusable, testable, chainable. Use for complex filtering/validation.
 * 
 * SCENARIO-BASED:
 * 16. Design a notification system supporting email, SMS, push notifications.
 *     Answer: Strategy pattern for notification methods. Factory to create strategies.
 *     Composite to send to multiple channels. Observer for event-driven notifications.
 * 
 * 17. You need to support multiple payment providers. Design approach.
 *     Answer: Strategy pattern for payment processing. Factory for provider selection.
 *     Adapter if providers have different interfaces. Repository for storing transactions.
 * 
 * 18. System needs undo/redo functionality. Design solution.
 *     Answer: Command pattern. Store commands in history stack. Each command implements
 *     Execute() and Undo(). Memento pattern for state snapshots if needed.
 * 
 * 19. Design a plugin architecture for extensible application.
 *     Answer: Define IPlugin interface. Use MEF or reflection to discover plugins.
 *     Factory to instantiate. Strategy for plugin selection. Observer for plugin events.
 * 
 * 20. Application needs to support multiple data sources. Design approach.
 *     Answer: Repository pattern with different implementations. Abstract Factory for
 *     creating repositories. Strategy for data source selection. Adapter if sources differ.
 * 
 * LEADERSHIP/ARCHITECTURE:
 * 21. How do you decide between monolith and microservices?
 *     Answer: Consider: team size, deployment needs, scalability, complexity, Conway's law.
 *     Start monolith, split to microservices when needed. Don't do microservices for sake of it.
 * 
 * 22. How do you enforce architecture boundaries in codebase?
 *     Answer: Project structure, ArchUnit tests, dependency analyzers, code reviews,
 *     documentation, onboarding, tooling (namespace rules, internal classes).
 * 
 * 23. Team disagrees on architecture approach. How do you resolve?
 *     Answer: Define criteria (performance, maintainability, cost). POC both approaches.
 *     Document trade-offs. Architecture Decision Records (ADRs). Vote or decide based on data.
 * 
 * 24. Legacy app needs modernization. Approach?
 *     Answer: Strangler pattern (gradual replacement). Anti-corruption layer. Identify seams.
 *     Extract bounded contexts. Feature flags. Parallel run. Incremental migration.
 * 
 * 25. How do you balance pragmatism vs perfect architecture?
 *     Answer: Consider: time, team skill, business value, technical debt. Quick wins vs
 *     long-term investment. Document shortcuts. Refactor incrementally. Pragmatism with plan.
 */
