using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD3.Models;

namespace TD3.Services.Seeder
{
    internal class ClientSeeder : ISeederService
    {
        private readonly ElectroShopContext _context;
        private readonly Faker<Client> _clientFaker;
        public int Order => 1;
        public ClientSeeder(ElectroShopContext context)
        {
            _context = context;

            _clientFaker = new Faker<Client>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.Email, f => f.Internet.Email());
        }

        public void SeedData()
        {
            Console.WriteLine("Seeding Clients");
            if (!_context.Clients.Any())
            {
                var clients = _clientFaker.Generate(20);
                _context.Clients.AddRange(clients);
                _context.SaveChanges();
            }
        }
    }
}
