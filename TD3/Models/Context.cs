using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace TD3.Models;

public partial class Context : DbContext
{
    public Context()
    {
    }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // Add way to determine the connection string based on the OS
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;");
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ElectroShop;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;");
            }
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.ClientId, "IX_Orders_ClientId");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders).HasForeignKey(d => d.ClientId);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_OrderLines_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderLines_ProductId");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines).HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Product).WithMany(p => p.OrderLines).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

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

            entity.HasOne(d => d.Product).WithOne(p => p.StockNavigation).HasForeignKey<Stock>(d => d.ProductId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
