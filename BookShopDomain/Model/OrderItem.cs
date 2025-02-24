using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace BookShopDomain.Model;

public partial class OrderItem : Entity
{
    [Required(ErrorMessage = "Необхідно вибрати товар")]
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Вкажіть кількість")]
    [Range(1, int.MaxValue, ErrorMessage = "Кількість повинна бути більше 0")]
    public int Quantity { get; set; }

    public int OrderId { get; set; }

    [ScaffoldColumn(false)]
    public virtual Order Order { get; set; } = null!;

    [ScaffoldColumn(false)]
    public virtual Product Product { get; set; } = null!;
}
