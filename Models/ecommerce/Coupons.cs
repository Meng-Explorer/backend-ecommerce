using System;
using System.Collections.Generic;

namespace WepApi.Models;

public class Coupons
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int UsageLimit { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<CouponUsages> CouponUsages { get; set; } = new List<CouponUsages>();
}