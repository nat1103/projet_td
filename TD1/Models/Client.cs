namespace TD1.Models;

public class Client
{
    public int ClientId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }

    // One-to-Many relationship with Order
    public ICollection<Order> Orders { get; set; }
}
