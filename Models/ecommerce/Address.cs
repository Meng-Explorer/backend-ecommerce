using System.Collections.Generic;

namespace WepApi.Models;

public class Address
{
    public int Id { get; set; }
    public AspNetUsers? User { get; set; }
    public string? UserId { get; set; }
    
    public string? RecipientName { get; set; }
    public string? Phone { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; }

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}