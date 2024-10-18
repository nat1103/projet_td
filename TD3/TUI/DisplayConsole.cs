using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TD3.Models;
using TD3.Services;


namespace TD3.TUI
{
    internal class DisplayConsole
    {
        private readonly ProductService productService;
        private readonly ClientService clientService;
        private readonly OrderService orderService;
        private readonly ElectroShopContext context;

        public DisplayConsole(ElectroShopContext electroShopContext)
        {
            productService = new ProductService(electroShopContext);
            clientService = new ClientService(electroShopContext);
            orderService = new OrderService(electroShopContext, productService);
            context = electroShopContext;
        }

        public Client DisplayCreateClient()
        {
            // Demander à l'utilisateur d'entrer les informations
            Console.WriteLine("Veuillez entrer l'email du client : ");
            string email = Console.ReadLine();

            Console.WriteLine("Veuillez entrer le nom du client : ");
            string nom = Console.ReadLine();

            Console.WriteLine("Veuillez entrer l'adresse du client : ");
            string adresse = Console.ReadLine();

            // Vérification des données
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Erreur : L'email fourni est invalide.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(nom))
            {
                Console.WriteLine("Erreur : Le nom ne peut pas être vide.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(adresse))
            {
                Console.WriteLine("Erreur : L'adresse ne peut pas être vide.");
                return null;
            }

            return new Client { Name = nom, Address = adresse, Email = email };
        }

        // Fonction pour vérifier si l'email est valide
        private static bool IsValidEmail(string email)
        {
            // Expression régulière pour valider un email simple
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        public void DisplayAllProducts()
        {
            foreach (var item in productService.GetProducts())
            {
                Console.WriteLine($"Product ID: {item.ProductId}, Name: {item.Name}, Price: {item.Price}, Stock: {item.Stock}");
            }
        }

        public void DisplayUpdateProduct()
        {
            Console.WriteLine("Veuillez entrer l'ID du produit à mettre à jour : ");
            if (!int.TryParse(Console.ReadLine(), out int productId) || productId <= 0)
            {
                Console.WriteLine("Erreur : ID du produit invalide.");
                return;
            }

            Console.WriteLine("Veuillez entrer le nom du produit : ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Erreur : Le nom du produit ne peut pas être vide.");
                return;
            }

            Console.WriteLine("Veuillez entrer le prix du produit : ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Erreur : Le prix doit être un nombre positif.");
                return;
            }

            Console.WriteLine("Veuillez entrer le stock du produit : ");
            if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
            {
                Console.WriteLine("Erreur : Le stock doit être un entier positif ou égal à zéro.");
                return;
            }

            // Essayer de mettre à jour le produit et capturer les erreurs éventuelles
            try
            {
                productService.UpdateProduct(productId, name, price, stock);
                Console.WriteLine("Le produit a été mis à jour avec succès !");
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Product not found"))
            {
                Console.WriteLine("Erreur : Le produit avec cet ID n'a pas été trouvé.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du produit : {ex.Message}");
            }
        }

        public string DisplayMenu()
        {
            Console.WriteLine(" 1/ TD4 : Initialisation de la base de données");
            Console.WriteLine(" 2/ TD5 : Mettre à jour un produits");
            Console.WriteLine(" 3/ TD6 : Transactions avec Entity Framework Core ");
            Console.WriteLine(" 4/ TD7 :  Lazy Loading et Eager Loading ");
            Console.WriteLine(" 5/ TD9 :  Procédures stockées : lecture de données");
            Console.WriteLine(" 6/ TD10 : Procédures stockées : écriture de données");

            Console.WriteLine("Veuillez entrer votre choix : ");
            return Console.ReadLine();
        }


        public void DisplayFailTransaction()
        {
            Console.WriteLine("Simulating a transaction...");

            var order1 = new Order
            {
                Date = DateTime.Now,
                ClientId = 11,
                Status = "Pending",
                OrderLines = new List<OrderLine>
            {
                new OrderLine { ProductId = 1, Quantity = 1 },
                new OrderLine { ProductId = 2, Quantity = 1 }
            }
            };

            orderService.ProcessOrder(order1);

            Console.WriteLine("All products:");
            productService.GetProducts();

            // Simulate a successful transaction
            Console.WriteLine("Simulating a unsuccessful transaction...");

            // Simulate a failed transaction for missing stock
            productService.UpdateProduct(1, "Laptop 2", 2000, 0);

            try
            {
                var order2 = new Order
                {
                    Date = DateTime.Now,
                    ClientId = 11,
                    Status = "Pending",
                    OrderLines = new List<OrderLine>
                {
                    new OrderLine { ProductId = 1, Quantity = 1 },
                    new OrderLine { ProductId = 2, Quantity = 1 }
                }
                };

                orderService.ProcessOrder(order2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction failed: {ex.Message}");
            }
        }

        public void DisplayAllUserWithOrder()
        {
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
                        //Console.WriteLine($"\t\tOrder line: {orderLine.ProductId} - {orderLine.Quantity}");
                        // Get product from product service
                        var product = productService.GetProductById(orderLine.ProductId);
                        Console.WriteLine($"\t\tOrder line: {product.Name} - {orderLine.Quantity}");
                    }
                }
                if (orders.Count == 0)
                {
                    Console.WriteLine("\tNo orders");
                }
            }
        }

        public void DisplayOrderByIdClient()
        {
            Console.WriteLine("Veuillez entrer l'ID du Client : ");
            if (!int.TryParse(Console.ReadLine(), out int orderId) || orderId <= 0)
            {
                Console.WriteLine("Erreur : ID de la client.");
                return;
            }

            var order = context.Orders.Include(o => o.OrderLines).FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                Console.WriteLine("Erreur : Commande introuvable.");
                return;
            }

            Console.WriteLine($"Order ID: {order.OrderId}, Date: {order.Date}, Status: {order.Status}");
            foreach (var orderLine in order.OrderLines)
            {
                var product = productService.GetProductById(orderLine.ProductId);
                Console.WriteLine($"\tProduct: {product.Name}, Quantity: {orderLine.Quantity}");
            }
        }
        public void LazyLoadingVsEagerLoading()
        {
            Console.WriteLine("Lazy Loading vs Eager Loading");
            var stopwatch = new Stopwatch();

            // Lazy Loading
            stopwatch.Start();
            var orders = context.Orders.ToList();
            var numberLazy = 0;

            if (orders != null)
            {
                foreach (var cmd in orders)
                {
                    foreach (var ligne in cmd.OrderLines)
                    {
                        Console.WriteLine($"Produit: {ligne.ProductId}, Quantité: {ligne.Quantity} Lazy");
                        numberLazy++;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Lazy Loading Time: {stopwatch.ElapsedMilliseconds} ms, Nombre de requêtes : {numberLazy}");
            Console.WriteLine("-----------------------------------------------------");

            // Eager Loading
            stopwatch.Reset();
            stopwatch.Start();
            var commandes = context.Orders.Include(c => c.OrderLines).ToList();
            var numberEager = 0;

            foreach (var cmd in commandes)
            {
                foreach (var ligne in cmd.OrderLines)
                {
                    Console.WriteLine($"Produit: {ligne.ProductId}, Quantité: {ligne.Quantity} Eager");
                    numberEager++;
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Eager Loading Time: {stopwatch.ElapsedMilliseconds} ms, Nombre de requêtes : {numberEager}");
        }

        public void DisplayGetOrdersByClient()
        {
            Console.WriteLine("Veuillez entrer l'ID du client : ");
            if (!int.TryParse(Console.ReadLine(), out int clientId) || clientId <= 0)
            {
                Console.WriteLine("Erreur : ID du client invalide.");
                return;
            }

            foreach (var order in orderService.GetOrdersByClient(clientId))
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Date: {order.Date}, Status: {order.Status}");
            }
        }

        public int DisplayAddOrder()
        {
            var listOrder = new List<OrderLine>();
            Console.WriteLine("Veuillez entrer l'ID du client : ");
            if (!int.TryParse(Console.ReadLine(), out int clientId) || clientId <= 0)
            {
                Console.WriteLine("Erreur : ID du client invalide.");
                return -1;
            }

            Order order = new Order
            {
                Date = DateTime.Now,
                ClientId = clientId,
                Status = "Pending"
            };

            while (true)
            {
                Console.WriteLine("Veuillez entrer l'ID du produit: ");
                if (!int.TryParse(Console.ReadLine(), out int productId) || productId <= 0)
                {
                    Console.WriteLine("Erreur : ID du produit invalide.");
                    return -1;
                }

                Console.WriteLine("Veuillez entrer la quantité : ");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Erreur : La quantité doit être un entier positif.");
                    return -1;
                }
                try
                {

                    foreach (var item in orderService.GetOrdersByClient(clientId))
                    {
                        Console.WriteLine(item.ToString());
                    }
                    var product = productService.GetProductById(productId);

                    if (product == null)
                    {
                        Console.WriteLine("Erreur : Produit introuvable.");
                        return -1;
                    }

                    listOrder.Add(new OrderLine
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price * quantity
                    });
                    Console.WriteLine("Produit ajouté à la commande.");


                    Console.WriteLine("Voulez-vous ajouter un autre produit ? (O/N)");
                    if (Console.ReadLine().ToUpper() != "O")
                    {
                        orderService.CreateOrderWithLines(order, listOrder);
                        foreach (var item in orderService.GetOrdersByClient(clientId))
                        {
                            Console.WriteLine(item.ToString());
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'ajout du produit : {ex.Message}");
                }
            }
            return -1;
        }

    }
}
