using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetRoleClaims : IdentityRoleClaim<string>
{
    public virtual AspNetRoles Role { get; set; } = null!;
}
