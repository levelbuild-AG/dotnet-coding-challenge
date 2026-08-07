using Microsoft.EntityFrameworkCore;
using CustomerApi.Models;

namespace CustomerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // These two lines tell EF Core: "make a table for Customers, and a table for Users"
    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer.Name must be unique — no two customers can have the same name
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Name)
            .IsUnique();

        // User.DisplayName must be unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.DisplayName)
            .IsUnique();

        // User.Email must be unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // If a Customer is deleted, delete all their Users too (cascade delete)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Customer)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}