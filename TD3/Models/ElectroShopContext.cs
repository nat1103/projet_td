using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace TD3.Models;

public partial class ElectroShopContext : DbContext
{
    public ElectroShopContext()
    {
    }

    public ElectroShopContext(DbContextOptions<ElectroShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }
    
    public virtual DbSet<VCommandesAvecTotal> VCommandesAvecTotal { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // Add way to determine the connection string based on the OS
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;")
                    .EnableSensitiveDataLogging()     // Enable sensitive data logging (optional)
                    .EnableDetailedErrors()        // Enable detailed errors (optional)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ElectroShop;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;")
                    .EnableSensitiveDataLogging()     // Enable sensitive data logging (optional)
                    .EnableDetailedErrors()        // Enable detailed errors (optional)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }

            optionsBuilder.UseLazyLoadingProxies(); // Add this line
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    
    {
        modelBuilder.Entity<VCommandesAvecTotal>(entity =>
        {
            entity.HasNoKey();  // Spécifie que cette entité n'a pas de clé
            entity.ToView("V_CommandesAvecTotal");  // Mappage vers la vue SQL
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");  // Spécifie le type de données pour "Total"
        });
        
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.ClientId, "IX_Orders_ClientId");

            entity.Property(e => e.Status) // Max length and required for "Status"
                  .HasMaxLength(20)
                  .IsRequired();

            entity.HasOne(d => d.Client).WithMany(p => p.Orders).HasForeignKey(d => d.ClientId);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_OrderLines_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderLines_ProductId");

            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();
            
            // Add check constraint for "UnitPrice"
            entity.HasCheckConstraint("CK_OrderLine_UnitPrice", "UnitPrice >= 0.01");

            entity.Property(e => e.Quantity)
                  .IsRequired()
                  .HasDefaultValue(1); // Default value for "Quantity" is 1
            
            // Add check constraint for "Quantity"
            entity.HasCheckConstraint("CK_OrderLine_Quantity", "Quantity >= 1");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines).HasForeignKey(d => d.OrderId)
                  .OnDelete(DeleteBehavior.Cascade); // cascade delete for "Order"

            entity.HasOne(d => d.Product).WithMany(p => p.OrderLines).HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // restrict delete for "Product"
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Price)
                  .HasColumnType("decimal(18, 2)")
                  .IsRequired();
            
            // Add check constraint for "Price"
            entity.HasCheckConstraint("CK_Product_Price", "Price >= 0");

            entity.Property(e => e.Name) // Ajout de la longueur maximale pour "Name"
                  .HasMaxLength(100)
                  .IsRequired(); // Rend le champ "Name" obligatoire

            entity.HasMany(d => d.Suppliers).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductSupplier",
                    r => r.HasOne<Supplier>().WithMany().HasForeignKey("SupplierId"),
                    l => l.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                    j =>
                    {
                        j.HasKey("ProductId", "SupplierId");
                        j.ToTable("ProductSuppliers");
                        j.HasIndex(new[] { "SupplierId" }, "IX_ProductSuppliers_SupplierId");
                    });
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.Property(e => e.QuantityInStock)
                  .IsRequired();

            entity.HasOne(d => d.Product).WithOne(p => p.StockNavigation).HasForeignKey<Stock>(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade); // cascade delete for "Product"
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Name) 
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Contact)
                  .HasMaxLength(100)
                  .IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }
    
    

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
