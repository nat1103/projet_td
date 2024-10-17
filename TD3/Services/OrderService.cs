using Microsoft.EntityFrameworkCore;
using TD3.Models;

namespace TD3.Services;

public class OrderService
{
    private readonly ElectroShopContext _electroShopContext;
    
    public OrderService(ElectroShopContext electroShopContext)
    {
        _electroShopContext = electroShopContext;
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