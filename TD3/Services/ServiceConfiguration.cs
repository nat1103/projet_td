using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD3.Models;
using TD3.Services.Seeder;
using Microsoft.Extensions.Logging;

namespace TD3.Services
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddElectroShopServices(this IServiceCollection services, string connectionString)
        {
            services
                .AddDbContext<ElectroShopContext>(options =>
                    options.UseSqlServer(connectionString)
                           .EnableSensitiveDataLogging()
                           .EnableDetailedErrors()
                           .LogTo(Console.WriteLine, LogLevel.Information)
                           .UseLazyLoadingProxies())
                .AddTransient<ProductService>()
                .AddTransient<ClientService>()
                .AddTransient<OrderService>()
                .AddTransient<ISeederService, ProductSeeder>()
                .AddTransient<ISeederService, ClientSeeder>()
                .AddTransient<ISeederService, OrderSeeder>()
                .AddTransient<DatabaseSeeder>();

            return services;
        }
    }
}
