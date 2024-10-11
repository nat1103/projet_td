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
        private readonly Context _context;
        public ClientService(Context context)
        {
            _context = context;
        }

        public Client AddClient(String name, String address, String email)
        {
            var client = new Client
            {
                Name = name,
                Address = address,
                Email = email
            };

            _context.Add(client);
            _context.SaveChanges();
            return client;
        }
        
        public void AddOrderForClient(int clientId, Order order)
        {
            var client = _context.Clients.Include(c => c.Orders).FirstOrDefault(c => c.ClientId == clientId);
            if (client != null)
            {
                client.Orders.Add(order);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Client not found");
            }
        }

        public void AddClients(IEnumerable<Client> clients)
        {
            _context.AddRange(clients);
            _context.SaveChanges();
        }
        
        // Get all clients
        public IEnumerable<Client> GetClients()
        {
            return _context.Clients.Include(c => c.Orders).ToList();
        }

    }
}
