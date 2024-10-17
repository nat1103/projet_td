using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices; // Add this using directive
using TD3.Models;
using TD3.Services;
using TD3.Services.Seeder;

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
        var serviceProvider = CreateService(connectionString);

        LazyLoadingVsEagerLoading(serviceProvider);

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

    // Seed the database with fake data
    private static void FakeData(ServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            seeder.Seed();
        }

        Console.WriteLine("Données factices insérées.");
    }

    private static ServiceProvider CreateService(String connectionString)
    {
        return new ServiceCollection()
            .AddDbContext<ElectroShopContext>(options =>
                options.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .UseLazyLoadingProxies()
               )
            .AddTransient<ProductService>()
            .AddTransient<ClientService>()
            .AddTransient<TransactionService>()
            .AddTransient<ISeederService, ProductSeeder>()
            .AddTransient<ISeederService, ClientSeeder>()
            .AddTransient<ISeederService, OrderSeeder>()
            .AddTransient<DatabaseSeeder>()
            .BuildServiceProvider();
            
    }
       
    private static void LazyLoadingVsEagerLoading(ServiceProvider serviceProvider)
    {
        var stopwatch = new Stopwatch();
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ElectroShopContext>();

            // Lazy Loading
            stopwatch.Start();
            var order = context.Orders.ToList();
            var numberLazy = 0;
            var number = 0;
            if (order != null)
            {
                foreach (var cmd in order)
                {
                    foreach (var ligne in cmd.OrderLines)
                    {
                        Console.WriteLine($"Produit: {ligne.ProductId}, Quantité: {ligne.Quantity} Lazy");
                        numberLazy++;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Lazy Loading Time: {stopwatch.ElapsedMilliseconds} ms , nombre de requêtes : {numberLazy}");
            Console.WriteLine("-----------------------------------------------------");
            // Eager Loading
            stopwatch.Reset();
            stopwatch.Start();
            var commandes = context.Orders.Include(c => c.OrderLines).ToList();
            foreach (var cmd in commandes)
            {
                foreach (var ligne in cmd.OrderLines)
                {
                    Console.WriteLine($"Produit: {ligne.ProductId}, Quantité: {ligne.Quantity} Eager");
                    number++;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Eager Loading Time: {stopwatch.ElapsedMilliseconds} ms, Nombre de requêtes : {number}");
        }
    }
}
