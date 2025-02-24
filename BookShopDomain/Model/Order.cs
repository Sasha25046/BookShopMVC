using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShopDomain.Model;

public partial class Order : Entity
{
    public int StatusId { get; set; }

    public int BuyerId { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Невірний формат дати для замовлення (yyyy-MM-dd).")]
    public DateOnly? OrderDate { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Невірний формат дати для доставки (yyyy-MM-dd).")]
    public DateOnly? DeliveryDate { get; set; }

    public virtual Buyer Buyer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Status Status { get; set; } = null!;
}
