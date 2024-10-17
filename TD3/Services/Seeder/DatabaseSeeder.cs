using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD3.Services.Seeder
{
    internal class DatabaseSeeder
    {
        private readonly IEnumerable<ISeederService> _seederServices;

        public DatabaseSeeder(IEnumerable<ISeederService> seederServices)
        {
            _seederServices = seederServices;
        }

        public void Seed()
        {
            Console.WriteLine("Seeding database...");
            
            foreach (var seederService in _seederServices)
            {

                seederService.SeedData();
            }
        }
    }
}
