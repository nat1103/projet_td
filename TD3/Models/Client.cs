using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TD3.Models;

public partial class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
