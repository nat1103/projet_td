using System;
using System.Collections.Generic;

namespace TD3.Models;

public partial class Stock
{
    public int ProductId { get; set; }

    public int QuantityInStock { get; set; }

    public virtual Product Product { get; set; } = null!;
}
