using Microsoft.EntityFrameworkCore;
using Databasprojekt.Models;

namespace Databasprojekt;

public class ShopContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRow> OrderRows => Set<OrderRow>();
    public DbSet<OrderSummary> OrderSummaries => Set<OrderSummary>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "databasprojekt.db");
        
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //OrderSummary
        modelBuilder.Entity<OrderSummary>(e =>
        {
            e.HasNoKey(); // No PK
            e.ToView("OrderSummaryView"); // Connects table to SQlite
            
        });
        //Categories
        modelBuilder.Entity<Category>(e =>
        {
            //PK
            e.HasKey(x => x.CategoryId);
            
            e.Property(x => x.CategoryName).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(250);
            
            e.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId); 
            
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Price).IsRequired();
            e.Property(x => x.StockQuantity).IsRequired();
            e.Property(x => x.Description).HasMaxLength(250);
            
        });

        modelBuilder.Entity<Customer>(e =>
        {
            //PK
            e.HasKey(x => x.CustomerId);
            
            //Properties
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(100);
            e.Property(x => x.City).HasMaxLength(100);
            
            //Unique Email
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Order>(e =>
        {
            //PK
            e.HasKey(x => x.OrderId);
            
            e.Property(x => x.OrderDate).IsRequired();
            e.Property(x => x.TotalAmount).IsRequired();
            e.Property(x => x.Status).IsRequired().HasMaxLength(100);
            
            e.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderRow>(e =>
        {
            //PK
            e.HasKey(x => x.OrderRowId);
            
            e.Property(x => x.Quantity).IsRequired();
            e.Property(x => x.UnitPrice).IsRequired();
            
            e.HasOne(x => x.Order)
                .WithMany(x => x.OrderRows)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}