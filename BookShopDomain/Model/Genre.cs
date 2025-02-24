using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BookShopDomain.Model;

public partial class Genre : Entity
{
    [Required(ErrorMessage = "Необхідно заповнити поле")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
