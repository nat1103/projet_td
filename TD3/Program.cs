using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TD3.Models;
using TD3.Services;
using TD3.Services.Seeder;
using TD3.TUI;

class Program
{

    public static void Main(string[] args)
    {
        Choice();
    }

    private ServiceProvider serviceProvider;
    private static ServiceProvider InitService() {
        var connectionString = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nathan\\Documents\\cours\\C#\\projet_td\\TD1\\Data\\bdd.mdf;Integrated Security=True;";
        }
        else
        {
            connectionString = "Server=localhost,1433;Database=ElectroShop;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;";
        }
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddElectroShopServices(connectionString);
        return serviceCollection.BuildServiceProvider();
    }


    private static void Choice()
    {
        DisplayConsole displayConsole = new DisplayConsole(InitService().GetRequiredService<ElectroShopContext>());

        while (true) // Boucle continue pour permettre à l'utilisateur de répéter les choix
        {
            String choice = displayConsole.DisplayMenu();
            Console.WriteLine("tt" + choice);
            switch (choice)
            {
                case "1":
                    FakeData(InitService());
                    break;
                case "2":
                    displayConsole.DisplayAllProducts();
                    displayConsole.DisplayUpdateProduct();
                    break;
                case "3":
                    displayConsole.DisplayFailTransaction();
                    displayConsole.DisplayAllUserWithOrder();
                    break;
                case "4":
                    displayConsole.LazyLoadingVsEagerLoading();
                    break;
                case "5":
                    displayConsole.DisplayGetOrdersByClient();
                    break;
                case "6":
                    displayConsole.DisplayAddOrder();
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }

            Console.WriteLine("Voulez-vous continuer ? (O/N)");
            String answer = Console.ReadLine().Trim().ToUpper();

            if (answer == "N")
            {
                Console.WriteLine("Au revoir !");
                break; // Sortir de la boucle, ce qui termine la fonction Choice()
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
    
}
