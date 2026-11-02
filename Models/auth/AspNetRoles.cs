using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetRoles : IdentityRole
{
    public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; } = new List<AspNetUserRoles>();
    public virtual ICollection<AspNetRoleClaims> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaims>();
}
