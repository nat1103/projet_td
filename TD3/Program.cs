using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices; // Add this using directive
using TD3.Models;
using TD3.Services;

class Program
{
    static void Main(string[] args)
    {
        // Determine the database connection string based on the OS
        string connectionString;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;";
        }
        else
        {
            connectionString = "Server=localhost,1433;Database=ElectroShop;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;";
        }

        // Configuration des services et du DbContext
        var serviceProvider = new ServiceCollection()
            .AddDbContext<Context>(options =>
                options.UseSqlServer(connectionString))  // Use the determined connection string
            .AddTransient<ProductService>()  // Ajout du service pour les produits
            .AddTransient<ClientService>()   // Ajout du service pour les clients
            .BuildServiceProvider();

        // Obtention des services
        var produitService = serviceProvider.GetService<ProductService>();
        var clientService = serviceProvider.GetService<ClientService>();

        // Ajouter des clients
        clientService.AddClient("Nathan", "3 rue de la paix", "test@test.fr");
        clientService.AddClient("Jean", "5 rue de la liberté", "jean@jean.fr");
        
        
        // Usage example
        produitService.AddProduct("Laptop", 1000, 10);
        produitService.AddProduct("Desktop", 800, 5);
        // Get all products to console
        Console.WriteLine("All products:");
        produitService.GetProducts();
        // Update product
        produitService.UpdateProduct(new Product { ProductId = 1, Name = "Laptop 2", Price = 2000, Stock = 15 });
        // Get all products to console
        Console.WriteLine("\nAll products after update:");
        produitService.GetProducts();
    }
}
