using System;
using System.Collections.Generic;

namespace WepApi.Models;

public class Orders
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    public AspNetUsers? User { get; set; }
    public string? UserId { get; set; }
    
    public Address? Address { get; set; }
    public int? AddressId { get; set; }
    
    public string? AddressSnapshot { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();
    public virtual ICollection<CouponUsages> CouponUsages { get; set; } = new List<CouponUsages>();
}