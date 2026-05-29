using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DarazClone.Models;

namespace DarazClone.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Seed categories using actual class, not anonymous object
            b.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", IconClass = "bi-phone" },
                new Category { Id = 2, Name = "Fashion", IconClass = "bi-bag" },
                new Category { Id = 3, Name = "Home", IconClass = "bi-house" }
            );
        }
    }
}