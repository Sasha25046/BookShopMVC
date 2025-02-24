using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShopDomain.Model;

public partial class Product : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Range(1400, int.MaxValue, ErrorMessage = "Рік повинен бути більше або дорівнювати 1400")]
    public int Year { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більшою за нуль")]
    public decimal Price { get; set; }

    public int SellerId { get; set; }

    public int BookStatusId { get; set; }

    public virtual BookStatus BookStatus { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Seller Seller { get; set; } = null!;

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
