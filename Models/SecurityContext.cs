using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstRestaurantAPI.Models
{
    public class SecurityContext: IdentityDbContext
    {
            public SecurityContext(DbContextOptions<SecurityContext> options) : base(options) { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);
            }

    }
}
