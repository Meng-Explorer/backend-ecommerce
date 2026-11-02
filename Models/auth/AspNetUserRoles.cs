using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetUserRoles : IdentityUserRole<string>
{
    public virtual AspNetRoles Role { get; set; } = null!;
    public virtual AspNetUsers User { get; set; } = null!;
}
