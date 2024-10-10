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

        public void AddClient(String name, String address, String email)
        {
            var client = new Client
            {
                Name = name,
                Address = address,
                Email = email
            };

            _context.Add(client);
            _context.SaveChanges();
        }

        public void addClients(IEnumerable<Client> clients)
        {
            _context.AddRange(clients);
            _context.SaveChanges();
        }

    }
}
