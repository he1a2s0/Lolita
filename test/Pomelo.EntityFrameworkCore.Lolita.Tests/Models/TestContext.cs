
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.Lolita.Tests.Models
{
    public class TestContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Customer> Customers { get; set; }
    }
}
