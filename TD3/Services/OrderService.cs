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
        return _electroShopContext.Orders.FromSqlRaw("EXEC GetCommandesByClient @ClientId = {0}", clientId).ToList();
    }
    
    
}