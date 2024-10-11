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

        public Client AddClient(String name, String address, String email)
        {
            var client = new Client
            {
                Name = name,
                Address = address,
                Email = email
            };

            _electroShopContext.Add(client);
            _electroShopContext.SaveChanges();
            return client;
        }
        
        public void AddOrderForClient(int clientId, Order order)
        {
            var client = _electroShopContext.Clients.Include(c => c.Orders).FirstOrDefault(c => c.ClientId == clientId);
            if (client != null)
            {
                client.Orders.Add(order);
                _electroShopContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Client not found");
            }
        }

        public void AddClients(IEnumerable<Client> clients)
        {
            _electroShopContext.AddRange(clients);
            _electroShopContext.SaveChanges();
        }
        
        // Get all clients
        public IEnumerable<Client> GetClients()
        {
            return _electroShopContext.Clients.Include(c => c.Orders).ToList();
        }

    }
}
