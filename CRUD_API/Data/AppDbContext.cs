using CRUD_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, FullName = "Ada Lovelace", Email = "ada@tech.com" },
                new Customer { Id = 2, FullName = "Alan Turing", Email = "alan@crypto.com" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Wireless Keyboard", Price = 49.99m },
                new Product { Id = 2, Name = "Gaming Mouse", Price = 59.99m }
            );
        }
    }
}
