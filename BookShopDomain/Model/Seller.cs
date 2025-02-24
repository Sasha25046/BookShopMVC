using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShopDomain.Model;

public partial class Seller : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
