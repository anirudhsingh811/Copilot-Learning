using Microsoft.EntityFrameworkCore;

namespace Single_Sign_On;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // OpenIddict will automatically create its tables
}
