using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetUserClaims : IdentityUserClaim<string>
{
    public virtual AspNetUsers User { get; set; } = null!;
}
