using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD3.Models;

namespace TD3.Services
{
    internal class ClientService
    {
        private readonly ElectroShopContext _electroShopContext;
        public ClientService(ElectroShopContext electroShopContext)
        {
            _electroShopContext = electroShopContext;
        }

        public void AddClient(String name, String address, String email)
        {
            var client = new Client
            {
                Name = name,
                Address = address,
                Email = email
            };

            _electroShopContext.Add(client);
            _electroShopContext.SaveChanges();
        }

        public void addClients(IEnumerable<Client> clients)
        {
            _electroShopContext.AddRange(clients);
            _electroShopContext.SaveChanges();
        }

    }
}
