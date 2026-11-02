using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetUserTokens : IdentityUserToken<string>
{
    public virtual AspNetUsers User { get; set; } = null!;
}
