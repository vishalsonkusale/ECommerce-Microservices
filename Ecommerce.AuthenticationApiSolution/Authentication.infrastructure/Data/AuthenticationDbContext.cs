using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.infrastructure.Data
{
    public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
    }
}
