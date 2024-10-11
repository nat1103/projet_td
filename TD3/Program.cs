using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices; // Add this using directive
using TD3.Models;
using TD3.Services;

// Program.cs
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
                options.UseSqlServer(connectionString))
            .AddTransient<ProductService>()
            .AddTransient<ClientService>()
            .AddTransient<TransactionService>()
            .BuildServiceProvider();

        var produitService = serviceProvider.GetService<ProductService>();
        var clientService = serviceProvider.GetService<ClientService>();
        var transactionService = serviceProvider.GetService<TransactionService>();
        
        Client nathanClient = clientService.AddClient("Nathan", "3 rue de la paix", "test@test.fr");
        Client jeanClient = clientService.AddClient("Jean", "5 rue de la liberté", "jean@jean.fr");

        Product laptop = produitService.AddProduct("Laptop", 1000, 10);
        Product desktop = produitService.AddProduct("Desktop", 800, 5);
        
        Console.WriteLine("Simulating a transaction...");

        var order1 = new Order
        {
            Date = DateTime.Now,
            ClientId = nathanClient.ClientId,
            Status = "Pending",
            OrderLines = new List<OrderLine>
            {
                new OrderLine { ProductId = laptop.ProductId, Quantity = 1 },
                new OrderLine { ProductId = desktop.ProductId, Quantity = 1 }
            }
        };
        
        transactionService.ProcessOrder(order1);
        
        Console.WriteLine("All products:");
        produitService.GetProducts();
        
        // Simulate a successful transaction
        Console.WriteLine("Simulating a unsuccessful transaction...");
        
        // Simulate a failed transaction for missing stock
        produitService.UpdateProduct(laptop.ProductId, "Laptop 2", 2000, 0);

        try
        {
            var order2 = new Order
            {
                Date = DateTime.Now,
                ClientId = jeanClient.ClientId,
                Status = "Pending",
                OrderLines = new List<OrderLine>
                {
                    new OrderLine { ProductId = laptop.ProductId, Quantity = 1 },
                    new OrderLine { ProductId = desktop.ProductId, Quantity = 1 }
                }
            };

            transactionService.ProcessOrder(order2);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Transaction failed: {ex.Message}");
        }
        
        // List all user and there orders
        Console.WriteLine("\nAll users and their orders:");
        var users = clientService.GetClients();
        foreach (var user in users)
        {
            Console.WriteLine($"User: {user.Name}");
            var orders = user.Orders;
            foreach (var order in orders)
            {
                Console.WriteLine($"\tOrder: {order.Date} - {order.Status}");
                foreach (var orderLine in order.OrderLines)
                {
                    Console.WriteLine($"\t\tOrder line: {orderLine.ProductId} - {orderLine.Quantity}");
                }
            }
            if (orders.Count == 0)
            {
                Console.WriteLine("\tNo orders");
            }
        }
       
    }
}
