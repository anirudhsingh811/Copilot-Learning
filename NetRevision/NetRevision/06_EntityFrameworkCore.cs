namespace NetRevision.DataAccess;

/// <summary>
/// POC: Entity Framework Core Patterns and Best Practices
/// Data access patterns for modern .NET applications
/// </summary>
public class EntityFrameworkPatterns
{
    public static void RunDemo()
    {
        Console.WriteLine("=== Entity Framework Core Patterns Demo ===\n");

        // 1. DbContext Configuration
        DbContextConfigurationDemo();

        // 2. Relationships
        RelationshipsDemo();

        // 3. Query Optimization
        QueryOptimizationDemo();

        // 4. Change Tracking
        ChangeTrackingDemo();

        // 5. Repository Pattern
        RepositoryPatternDemo();

        // 6. Unit of Work Pattern
        UnitOfWorkPatternDemo();
    }

    #region 1. DbContext Configuration
    private static void DbContextConfigurationDemo()
    {
        Console.WriteLine("1. DbContext Configuration Pattern\n");

        Console.WriteLine("Example DbContext:");
        Console.WriteLine(@"
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity Configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(e => e.Price)
                  .HasPrecision(18, 2);
            entity.HasIndex(e => e.Sku)
                  .IsUnique();
        });

        // Global Query Filter (Soft Delete)
        modelBuilder.Entity<Product>()
                    .HasQueryFilter(p => !p.IsDeleted);

        // Table Splitting / TPH / TPT
        modelBuilder.Entity<Customer>()
                    .ToTable(""Customers"");

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
");
        Console.WriteLine("Key Configuration Points:");
        Console.WriteLine("? Entity properties (required, max length, precision)");
        Console.WriteLine("? Indexes for performance");
        Console.WriteLine("? Global query filters (soft delete, multi-tenant)");
        Console.WriteLine("? Table mapping strategies\n");
    }
    #endregion

    #region 2. Relationships
    private static void RelationshipsDemo()
    {
        Console.WriteLine("2. Entity Relationships\n");

        Console.WriteLine("=== One-to-Many Relationship ===");
        Console.WriteLine(@"
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Navigation property
    public ICollection<Order> Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    
    // Foreign key
    public int CustomerId { get; set; }
    
    // Navigation property
    public Customer Customer { get; set; }
}

// Configuration
modelBuilder.Entity<Order>()
    .HasOne(o => o.Customer)
    .WithMany(c => c.Orders)
    .HasForeignKey(o => o.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);
");

        Console.WriteLine("\n=== Many-to-Many Relationship (EF Core 5+) ===");
        Console.WriteLine(@"
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Course> Courses { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public ICollection<Student> Students { get; set; }
}

// EF Core automatically creates join table StudentCourses
");

        Console.WriteLine("\n=== One-to-One Relationship ===");
        Console.WriteLine(@"
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserProfile Profile { get; set; }
}

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Bio { get; set; }
    public User User { get; set; }
}

modelBuilder.Entity<User>()
    .HasOne(u => u.Profile)
    .WithOne(p => p.User)
    .HasForeignKey<UserProfile>(p => p.UserId);
");
        Console.WriteLine();
    }
    #endregion

    #region 3. Query Optimization
    private static void QueryOptimizationDemo()
    {
        Console.WriteLine("3. Query Optimization Patterns\n");

        Console.WriteLine("? ANTI-PATTERN: N+1 Query Problem");
        Console.WriteLine(@"
var orders = await context.Orders.ToListAsync();
foreach (var order in orders)
{
    // This executes a separate query for each order!
    Console.WriteLine(order.Customer.Name);
}
// Result: 1 query for orders + N queries for customers = N+1
");

        Console.WriteLine("\n? SOLUTION 1: Eager Loading");
        Console.WriteLine(@"
var orders = await context.Orders
    .Include(o => o.Customer)
    .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
    .ToListAsync();
// Result: 1-2 queries (depending on split query)
");

        Console.WriteLine("\n? SOLUTION 2: Projection (Best for DTOs)");
        Console.WriteLine(@"
var orderDtos = await context.Orders
    .Select(o => new OrderDto
    {
        OrderId = o.Id,
        OrderDate = o.OrderDate,
        CustomerName = o.Customer.Name,
        TotalItems = o.OrderItems.Count
    })
    .ToListAsync();
// Result: 1 optimized query, no tracking overhead
");

        Console.WriteLine("\n? SOLUTION 3: Explicit Loading (when needed)");
        Console.WriteLine(@"
var order = await context.Orders.FindAsync(orderId);
await context.Entry(order)
    .Collection(o => o.OrderItems)
    .LoadAsync();
");

        Console.WriteLine("\n? AsNoTracking for Read-Only Queries");
        Console.WriteLine(@"
var products = await context.Products
    .AsNoTracking() // Better performance, no change tracking
    .Where(p => p.Price < 100)
    .ToListAsync();
");

        Console.WriteLine("\n? Split Queries (EF Core 5+)");
        Console.WriteLine(@"
var orders = await context.Orders
    .Include(o => o.Customer)
    .Include(o => o.OrderItems)
    .AsSplitQuery() // Separate query for each Include
    .ToListAsync();
// Avoids cartesian explosion
");

        Console.WriteLine("\n? Compiled Queries (for frequently used queries)");
        Console.WriteLine(@"
private static readonly Func<AppDbContext, int, Task<Product>> 
    GetProductById = EF.CompileAsyncQuery(
        (AppDbContext context, int id) =>
            context.Products.FirstOrDefault(p => p.Id == id));

var product = await GetProductById(context, 123);
// Query plan cached, faster execution
");
        Console.WriteLine();
    }
    #endregion

    #region 4. Change Tracking
    private static void ChangeTrackingDemo()
    {
        Console.WriteLine("4. Change Tracking and State Management\n");

        Console.WriteLine("Entity States:");
        Console.WriteLine("• Detached - Not tracked by context");
        Console.WriteLine("• Unchanged - Tracked, no changes");
        Console.WriteLine("• Added - New entity, will be inserted");
        Console.WriteLine("• Modified - Tracked with changes, will be updated");
        Console.WriteLine("• Deleted - Marked for deletion\n");

        Console.WriteLine("Example Usage:");
        Console.WriteLine(@"
// Add new entity
var product = new Product { Name = ""Laptop"", Price = 999 };
context.Products.Add(product); // State: Added
await context.SaveChangesAsync(); // INSERT

// Update entity
var product = await context.Products.FindAsync(1);
product.Price = 1099; // State: Modified
await context.SaveChangesAsync(); // UPDATE

// Delete entity
var product = await context.Products.FindAsync(1);
context.Products.Remove(product); // State: Deleted
await context.SaveChangesAsync(); // DELETE

// Attach disconnected entity
var product = new Product { Id = 1, Name = ""Laptop"", Price = 1099 };
context.Attach(product);
context.Entry(product).State = EntityState.Modified;
await context.SaveChangesAsync();

// Track only specific properties
var product = await context.Products.FindAsync(1);
context.Entry(product).Property(p => p.Price).IsModified = true;
await context.SaveChangesAsync();
");
        Console.WriteLine();
    }
    #endregion

    #region 5. Repository Pattern
    private static void RepositoryPatternDemo()
    {
        Console.WriteLine("5. Repository Pattern\n");

        Console.WriteLine("Generic Repository Interface:");
        Console.WriteLine(@"
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }
}
");

        Console.WriteLine("\nSpecific Repository (for complex queries):");
        Console.WriteLine(@"
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
}

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _dbSet
            .Where(p => p.Stock < threshold)
            .ToListAsync();
    }
}
");

        Console.WriteLine("\n?? Repository Pattern Debate:");
        Console.WriteLine("PROS: Testability, abstraction, centralized data access");
        Console.WriteLine("CONS: EF Core is already repository/UoW, extra layer");
        Console.WriteLine("RECOMMENDATION: Use for complex queries, avoid generic repository overhead\n");
    }
    #endregion

    #region 6. Unit of Work Pattern
    private static void UnitOfWorkPatternDemo()
    {
        Console.WriteLine("6. Unit of Work Pattern\n");

        Console.WriteLine("Interface:");
        Console.WriteLine(@"
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
");

        Console.WriteLine("\nImplementation:");
        Console.WriteLine(@"
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICustomerRepository Customers { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new ProductRepository(_context);
        Orders = new OrderRepository(_context);
        Customers = new CustomerRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction?.RollbackAsync()!;
        _transaction?.Dispose();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
");

        Console.WriteLine("\nUsage in Service:");
        Console.WriteLine(@"
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateOrderAsync(OrderDto orderDto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(orderDto.CustomerId);
            var order = new Order { CustomerId = customer.Id, OrderDate = DateTime.UtcNow };
            
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            foreach (var item in orderDto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                product.Stock -= item.Quantity;
                await _unitOfWork.Products.UpdateAsync(product);
            }

            await _unitOfWork.CommitTransactionAsync();
            return order.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
");
        Console.WriteLine();
    }
    #endregion
}

/*
 * =========================================================
 * INTERVIEW QUESTIONS - Entity Framework Core
 * =========================================================
 * 
 * BASIC:
 * 1. What is Entity Framework Core and how does it differ from Entity Framework 6?
 *    Answer: EF Core is cross-platform, lightweight ORM. No EDMX, Code First preferred,
 *    better performance, supports NoSQL. Not all EF6 features ported.
 * 
 * 2. Explain Code First vs Database First approach.
 *    Answer: Code First: Define entities in code, generate database with migrations.
 *    Database First: Generate entities from existing database. Code First preferred for new projects.
 * 
 * 3. What is DbContext and DbSet?
 *    Answer: DbContext: Represents session with database, unit of work, change tracking.
 *    DbSet<T>: Collection of entities, queryable, represents table.
 * 
 * 4. What are migrations and how do they work?
 *    Answer: Version control for database schema. Add-Migration creates migration file.
 *    Update-Database applies. Can script for production deployment.
 * 
 * 5. Explain lazy loading vs eager loading.
 *    Answer: Lazy: Related data loaded on access (proxies). Eager: Use Include() to load upfront.
 *    Lazy can cause N+1, eager can load too much. Explicit loading also available.
 * 
 * INTERMEDIATE:
 * 6. What is the N+1 query problem and how do you solve it?
 *    Answer: 1 query for main entity + N queries for related entities. Solutions:
 *    Include() for eager loading, Select() for projection, AsSplitQuery() to avoid cartesian explosion.
 * 
 * 7. When should you use AsNoTracking()?
 *    Answer: Read-only queries, better performance (no change tracking overhead).
 *    DTOs, reporting, displaying data. Don't use when you need to update entities.
 * 
 * 8. Explain entity states in EF Core.
 *    Answer: Detached, Unchanged, Added, Modified, Deleted. Context tracks state.
 *    SaveChanges() generates SQL based on state (INSERT, UPDATE, DELETE).
 * 
 * 9. How do you handle concurrency conflicts?
 *    Answer: Optimistic concurrency with [Timestamp] or [ConcurrencyCheck].
 *    Catches DbUpdateConcurrencyException. Handle by: client wins, database wins, or merge.
 * 
 * 10. What are global query filters and when to use them?
 *     Answer: Automatically applied to all queries for entity type. Use for: soft delete,
 *     multi-tenant filtering, audit filtering. Can be ignored with IgnoreQueryFilters().
 * 
 * ADVANCED:
 * 11. Explain compiled queries and their benefits.
 *     Answer: EF.CompileQuery() or EF.CompileAsyncQuery() caches query plan. Faster execution
 *     for frequently used queries. Trade-off: memory for speed. Use for hot paths.
 * 
 * 12. How does EF Core handle transactions?
 *     Answer: SaveChanges() is implicit transaction. Use Database.BeginTransaction() for explicit.
 *     Can span multiple SaveChanges() calls. Support for distributed transactions (System.Transactions).
 * 
 * 13. What is query splitting and when should you use it?
 *     Answer: AsSplitQuery() separates query with multiple Includes into multiple SQL queries.
 *     Avoids cartesian explosion with multiple collections. Trade-off: more roundtrips vs data duplication.
 * 
 * 14. How do you optimize bulk operations in EF Core?
 *     Answer: AddRange/UpdateRange/RemoveRange for batching. Use libraries (EFCore.BulkExtensions)
 *     for large operations. Consider raw SQL for very large updates. Disable change tracking.
 * 
 * 15. Explain TPH, TPT, and TPC inheritance strategies.
 *     Answer: TPH (Table Per Hierarchy): One table with discriminator. TPT (Table Per Type):
 *     Table for each type. TPC (Table Per Concrete): No base table. TPH is default, fastest.
 * 
 * SCENARIO-BASED:
 * 16. You need to implement soft delete. How do you approach it?
 *     Answer: Add IsDeleted property. Global query filter: HasQueryFilter(e => !e.IsDeleted).
 *     Override SaveChanges() to set IsDeleted instead of removing. IgnoreQueryFilters() when needed.
 * 
 * 17. API is slow due to database queries. Investigation approach?
 *     Answer: Enable logging (LogTo). Use EF Core profiling. Check for N+1 queries.
 *     Add indexes. Use projections instead of full entities. Consider caching. Check query execution plans.
 * 
 * 18. Design multi-tenant database architecture with EF Core.
 *     Answer: Options: 1) Separate databases per tenant. 2) Shared database, separate schemas.
 *     3) Shared database, TenantId column. Use global query filter for tenant isolation.
 *     Resolve tenant from middleware/context.
 * 
 * 19. How do you handle database connection resilience?
 *     Answer: EnableRetryOnFailure() in DbContext options. Configure retry count and delay.
 *     Handle transient errors (network, timeout). Use circuit breaker pattern.
 * 
 * 20. Should you use Repository pattern with EF Core? Justify.
 *     Answer: Controversial. PROS: Testability, abstraction, swap ORM. CONS: EF Core is already
 *     repository/UoW, extra layer, harder to use EF features. RECOMMENDATION: Direct DbContext
 *     for simple apps, Repository for complex domain logic or switching ORMs.
 */
