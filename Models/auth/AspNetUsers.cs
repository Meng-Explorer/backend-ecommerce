using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetUsers : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Status { get; set; }
    public bool IsSuperUser { get; set; }
    public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; } = new List<AspNetUserClaims>();
    public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; } = new List<AspNetUserLogins>();
    public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; } = new List<AspNetUserTokens>();
    public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; } = new List<AspNetUserRoles>();

    // Relationships to eCommerce entities
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
    public virtual ICollection<Carts> Carts { get; set; } = new List<Carts>();
    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    public virtual ICollection<Wishlists> Wishlists { get; set; } = new List<Wishlists>();
    public virtual ICollection<CouponUsages> CouponUsages { get; set; } = new List<CouponUsages>();
}
