using System;

namespace WepApi.Models;

public class Payments
{
    public int Id { get; set; }
    public Orders? Order { get; set; }
    public int OrderId { get; set; }
    public string? Provider { get; set; }
    public string? TransactionRef { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}