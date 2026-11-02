using Microsoft.AspNetCore.Identity;

namespace WepApi.Models;

public partial class AspNetUserLogins : IdentityUserLogin<string>
{
    public virtual AspNetUsers User { get; set; } = null!;
}
