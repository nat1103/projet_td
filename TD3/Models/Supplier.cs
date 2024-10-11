using System;
using System.Collections.Generic;

namespace TD3.Models;

using System.ComponentModel.DataAnnotations;

public partial class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    [Required]
    [MaxLength(100)] // Limite le nom du fournisseur à 100 caractères
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)] // Limite le contact à 100 caractères
    public string Contact { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

