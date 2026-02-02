using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace AZ204.Monitoring.Cache;

/// <summary>
/// Real Azure Cache for Redis implementation
/// Demonstrates cache-aside pattern, distributed caching, and performance optimization
/// </summary>
public class RedisCacheService
{
    private readonly IDatabase _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly ConnectionMultiplexer _redis;

    public RedisCacheService(string connectionString, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _cache = _redis.GetDatabase();
        
        _logger.LogInformation("Redis connection established");
        Console.WriteLine("? Connected to Azure Cache for Redis");
    }

    /// <summary>
    /// Cache-Aside Pattern: Get or create cached item
    /// </summary>
    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> createItem,
        TimeSpan? expiration = null) where T : class
    {
        try
        {
            // Try to get from cache
            var cachedValue = await _cache.StringGetAsync(key);
            
            if (!cachedValue.IsNullOrEmpty)
            {
                _logger.LogInformation("Cache hit for key: {Key}", key);
                Console.WriteLine($"?? Cache HIT: {key}");
                
                return JsonSerializer.Deserialize<T>(cachedValue!);
            }

            // Cache miss - create and store
            _logger.LogInformation("Cache miss for key: {Key}", key);
            Console.WriteLine($"? Cache MISS: {key}");
            Console.WriteLine($"   ? Fetching from source...");
            
            var item = await createItem();
            
            if (item != null)
            {
                var serialized = JsonSerializer.Serialize(item);
                await _cache.StringSetAsync(key, serialized, expiration);
                
                Console.WriteLine($"   ? Cached for {expiration?.TotalMinutes ?? -1} minutes");
            }

            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache operation failed for key: {Key}", key);
            Console.WriteLine($"??  Cache error: {ex.Message}");
            
            // Fallback to source on cache failure
            return await createItem();
        }
    }

    /// <summary>
    /// Set cache value with expiration
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, serialized, expiration);
            
            _logger.LogInformation("Cached value for key: {Key}", key);
            Console.WriteLine($"?? Set cache: {key} (expires in {expiration?.TotalMinutes ?? -1} min)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set cache for key: {Key}", key);
            Console.WriteLine($"? Cache set failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Get cache value
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var cachedValue = await _cache.StringGetAsync(key);
            
            if (!cachedValue.IsNullOrEmpty)
            {
                Console.WriteLine($"? Retrieved from cache: {key}");
                return JsonSerializer.Deserialize<T>(cachedValue!);
            }

            Console.WriteLine($"? Key not found in cache: {key}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache for key: {Key}", key);
            Console.WriteLine($"? Cache get failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Delete cache value
    /// </summary>
    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            var deleted = await _cache.KeyDeleteAsync(key);
            
            if (deleted)
            {
                Console.WriteLine($"???  Deleted from cache: {key}");
            }
            else
            {
                Console.WriteLine($"??  Key not found: {key}");
            }
            
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete cache for key: {Key}", key);
            Console.WriteLine($"? Cache delete failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if key exists
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _cache.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check existence for key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Get time to live for a key
    /// </summary>
    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        try
        {
            var ttl = await _cache.KeyTimeToLiveAsync(key);
            
            if (ttl.HasValue)
            {
                Console.WriteLine($"??  TTL for {key}: {ttl.Value.TotalMinutes:F2} minutes");
            }
            else
            {
                Console.WriteLine($"??  Key {key} has no expiration");
            }
            
            return ttl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get TTL for key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Increment counter (useful for rate limiting, counters)
    /// </summary>
    public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null)
    {
        try
        {
            var newValue = await _cache.StringIncrementAsync(key, value);
            
            if (expiration.HasValue)
            {
                await _cache.KeyExpireAsync(key, expiration);
            }
            
            Console.WriteLine($"? Incremented {key}: {newValue}");
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to increment key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Get cache statistics
    /// </summary>
    public async Task<Dictionary<string, string>> GetCacheStatsAsync()
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var info = await server.InfoAsync();
            
            var stats = new Dictionary<string, string>();
            
            foreach (var section in info)
            {
                foreach (var item in section)
                {
                    stats[$"{section.Key}:{item.Key}"] = item.Value;
                }
            }

            Console.WriteLine("\n?? Redis Cache Statistics:");
            Console.WriteLine("???????????????????????????????????????");
            
            var importantStats = new[]
            {
                "Server:redis_version",
                "Server:uptime_in_seconds",
                "Stats:total_connections_received",
                "Stats:total_commands_processed",
                "Stats:keyspace_hits",
                "Stats:keyspace_misses",
                "Memory:used_memory_human",
                "Clients:connected_clients"
            };

            foreach (var key in importantStats)
            {
                if (stats.TryGetValue(key, out var value))
                {
                    Console.WriteLine($"  {key}: {value}");
                }
            }

            if (stats.TryGetValue("Stats:keyspace_hits", out var hits) &&
                stats.TryGetValue("Stats:keyspace_misses", out var misses))
            {
                var totalRequests = long.Parse(hits) + long.Parse(misses);
                if (totalRequests > 0)
                {
                    var hitRate = (double.Parse(hits) / totalRequests) * 100;
                    Console.WriteLine($"\n  Cache Hit Rate: {hitRate:F2}%");
                }
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache stats");
            Console.WriteLine($"? Failed to get stats: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Demonstrate cache-aside pattern with simulated database
    /// </summary>
    public async Task<Product> GetProductAsync(int productId)
    {
        var cacheKey = $"product:{productId}";
        
        return await GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                // Simulate database call
                Console.WriteLine($"   ?? Fetching from database...");
                await Task.Delay(Random.Shared.Next(500, 1500)); // Simulate DB latency
                
                return new Product
                {
                    Id = productId,
                    Name = $"Product {productId}",
                    Price = Random.Shared.Next(10, 1000),
                    Stock = Random.Shared.Next(0, 100),
                    LastUpdated = DateTime.UtcNow
                };
            },
            expiration: TimeSpan.FromMinutes(10)
        ) ?? new Product();
    }

    /// <summary>
    /// Demonstrate session state management
    /// </summary>
    public async Task ManageSessionAsync(string sessionId, UserSession session)
    {
        var cacheKey = $"session:{sessionId}";
        await SetAsync(cacheKey, session, TimeSpan.FromMinutes(30));
        Console.WriteLine($"?? Session stored for user: {session.UserId}");
    }

    /// <summary>
    /// Demonstrate rate limiting
    /// </summary>
    public async Task<bool> CheckRateLimitAsync(string userId, int maxRequests = 100, TimeSpan? window = null)
    {
        var windowSpan = window ?? TimeSpan.FromMinutes(1);
        var cacheKey = $"ratelimit:{userId}";
        
        var count = await IncrementAsync(cacheKey, 1, windowSpan);
        
        var allowed = count <= maxRequests;
        var icon = allowed ? "?" : "?";
        
        Console.WriteLine($"{icon} Rate limit check for {userId}: {count}/{maxRequests}");
        
        return allowed;
    }

    /// <summary>
    /// Clean up resources
    /// </summary>
    public void Dispose()
    {
        _redis?.Dispose();
        Console.WriteLine("?? Redis connection closed");
    }
}

// Models
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class UserSession
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
}
