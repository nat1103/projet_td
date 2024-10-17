using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TD3.Models;

namespace TD3.Services
{
    internal class OrderService
    {
        private readonly ElectroShopContext _electroShopContext;
        private readonly ProductService _productService;

        public OrderService(ElectroShopContext electroShopContext, ProductService productService)
        {
            _electroShopContext = electroShopContext;
            _productService = productService;
        }

        public void ProcessOrder(Order order)
        {
            using (var transaction = _electroShopContext.Database.BeginTransaction())
            {
                try
                {
                    // Save order and order lines
                    _electroShopContext.Orders.Add(order);
                    _electroShopContext.SaveChanges();
        
                    foreach (var orderLine in order.OrderLines)
                    {
                        var product = _electroShopContext.Products.FirstOrDefault(p => p.ProductId == orderLine.ProductId);
                        if (product == null || product.Stock <= 0)
                        {
                            throw new InvalidOperationException($"Product with ID: {orderLine.ProductId} is not available in stock.");
                        }
        
                        _productService.UpdateProduct(product.ProductId, product.Name, product.Price, product.Stock - orderLine.Quantity);
                    }
                    
                    order.Status = "Completed";
                    _electroShopContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    order.Status = "Failed";
                    _electroShopContext.SaveChanges();
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // Method to get all orders for a client using a stored procedure
        public List<Order> GetOrdersByClient(int clientId)
        {
            return _electroShopContext.Orders.FromSqlRaw("EXEC GetOrderByClient @ClientId = {0}", clientId).ToList();
        }

        // Method to add an order using a stored procedure
        public int AddOrder(int clientId, DateTime date, string status)
        {
            var orderIdParam = new Microsoft.Data.SqlClient.SqlParameter
            {
                ParameterName = "@OrderId",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            var clientIdParam = new Microsoft.Data.SqlClient.SqlParameter("@ClientId", clientId);
            var dateParam = new Microsoft.Data.SqlClient.SqlParameter("@Date", date);
            var statusParam = new Microsoft.Data.SqlClient.SqlParameter("@Status", status);

            // Exécute la procédure stockée avec le paramètre de sortie
            _electroShopContext.Database.ExecuteSqlRaw(
                "EXEC AddOrder @ClientId, @Date, @Status, @OrderId OUTPUT",
                clientIdParam, dateParam, statusParam, orderIdParam
            );

            // Récupère la valeur de l'ID généré
            return (int)orderIdParam.Value;
        }

        // Method to add an order line using a stored procedure
        public void AddOrderLine(int orderId, int productId, int quantity, decimal unitPrice)
        {
            _electroShopContext.Database.ExecuteSqlRaw("EXEC AddOrderLine @OrderID = {0}, @ProductID = {1}, @Quantity = {2}, @UnitPrice = {3}",
                orderId, productId, quantity, unitPrice);
        }

        // Method that creates an order and its order lines
        public int CreateOrderWithLines(Order order, List<OrderLine> orderLines)
        {
            if (orderLines.Count == 0)
            {
                throw new Exception("An order must have at least one order line");
            }
            if (orderLines.Any(line => line.Quantity <= 0) || orderLines.Any(line => line.UnitPrice <= 0))
            {
                throw new Exception("Order lines must have a positive quantity and a positive unit price");
            }
            try
            {
                // Add the order
                int newOrderId = this.AddOrder(order.ClientId, order.Date, order.Status);

                // Add the order lines
                foreach (var line in orderLines)
                {
                    this.AddOrderLine(newOrderId, line.ProductId, line.Quantity, line.UnitPrice);
                }

                return newOrderId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating order", ex);
            }
        }
    }
}