using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WepApi.Models;
using WepApi.Repository;

namespace WepApi.Data;

public class IdentitySeeder
{
    private static readonly string[] RequiredRoles = { "SuperAdmin", "Admin", "Manager", "User" };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");
            var config = sp.GetRequiredService<IConfiguration>();

            // Opt-in. MIS_WEBSITE_DB is shared with other applications, so the
            // app must not write to it on startup unless someone asked for it.
            if (!config.GetValue<bool>("Bootstrap:Enabled"))
            {
                logger.LogDebug("Bootstrap: disabled (Bootstrap:Enabled is not true); skipping.");
                return;
            }

            var roleRepo = sp.GetRequiredService<IRoleRepository>();
            var userRepo = sp.GetRequiredService<IUserRepository>();
            var userRoleRepo = sp.GetRequiredService<IUserRoleRepository>();
            var hasher = sp.GetRequiredService<PasswordHasher<AspNetUsers>>();
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();

            var (superAdminRole, adminRole) = await EnsureRolesAsync(roleRepo, logger);

            var username = config["Bootstrap:AdminUsername"];
            var password = config["Bootstrap:AdminPassword"];
            var email = config["Bootstrap:AdminEmail"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning(
                    "Bootstrap: Bootstrap:AdminUsername and Bootstrap:AdminPassword must be configured to seed the SuperAdmin account.");
                return;
            }

            username = username.Trim().ToLowerInvariant();
            email = string.IsNullOrWhiteSpace(email) ? username : email.Trim().ToLowerInvariant();

            var admin = await userRepo.GetUserByNameAsync(username)
                     ?? await userRepo.GetUserByEmailAsync(email);

            if (admin is null)
            {
                admin = new AspNetUsers
                {
                    UserName = username,
                    NormalizedUserName = username.ToUpperInvariant(),
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    FirstName = "System",
                    LastName = "SuperAdmin",
                    Status = "Active",
                    EmailConfirmed = true,
                    IsSuperUser = true
                };
                admin.PasswordHash = hasher.HashPassword(admin, password);
                await userRepo.Register(admin);
                logger.LogInformation("Bootstrap: created initial SuperAdmin account {Username}.", username);
            }
            else
            {
                admin.IsSuperUser = true;
                admin.Status = "Active";
                if (!string.IsNullOrWhiteSpace(password))
                {
                    admin.PasswordHash = hasher.HashPassword(admin, password);
                }
                if (!string.IsNullOrWhiteSpace(email))
                {
                    admin.Email = email;
                    admin.NormalizedEmail = email.ToUpperInvariant();
                }
                await userRepo.UpdateUserAsync(admin);
                logger.LogInformation("Bootstrap: ensured account {Username} is SuperAdmin with active status.", username);
            }

            // Assign SuperAdmin role to this user
            if (!await userRoleRepo.HashRoleAsync(admin.Id, superAdminRole.Id))
            {
                await userRoleRepo.AddUserRoleAsync(new AspNetUserRoles
                {
                    UserId = admin.Id,
                    RoleId = superAdminRole.Id
                });
            }

            // Also assign Admin role to this user
            if (adminRole != null && !await userRoleRepo.HashRoleAsync(admin.Id, adminRole.Id))
            {
                await userRoleRepo.AddUserRoleAsync(new AspNetUserRoles
                {
                    UserId = admin.Id,
                    RoleId = adminRole.Id
                });
            }

            await userRoleRepo.SaveChangesAsync();

            // Enforce: ONLY this user is SuperAdmin.
            // Demote any other accounts that hold the SuperAdmin role.
            var otherSuperAdminRoles = await dbContext.UserRoles
                .Where(ur => ur.RoleId == superAdminRole.Id && ur.UserId != admin.Id)
                .ToListAsync();

            if (otherSuperAdminRoles.Any())
            {
                dbContext.UserRoles.RemoveRange(otherSuperAdminRoles);
                logger.LogWarning("Bootstrap: removed SuperAdmin role from {Count} other account(s).", otherSuperAdminRoles.Count);
            }

            // Demote IsSuperUser flag for any other accounts.
            var otherSuperUsers = await dbContext.Users
                .Where(u => u.Id != admin.Id && u.IsSuperUser)
                .ToListAsync();

            if (otherSuperUsers.Any())
            {
                foreach (var other in otherSuperUsers)
                {
                    other.IsSuperUser = false;
                }
                logger.LogWarning("Bootstrap: stripped IsSuperUser flag from {Count} other account(s).", otherSuperUsers.Count);
            }

            if (otherSuperAdminRoles.Any() || otherSuperUsers.Any())
            {
                await dbContext.SaveChangesAsync();
            }

            logger.LogInformation("Bootstrap: only {Username} is now designated as SuperAdmin.", username);
        }

        private static async Task<(AspNetRoles SuperAdminRole, AspNetRoles AdminRole)> EnsureRolesAsync(IRoleRepository roleRepo, ILogger logger)
        {
            AspNetRoles? superAdminRole = null;
            AspNetRoles? adminRole = null;

            foreach (var roleName in RequiredRoles)
            {
                var role = await roleRepo.GetByNameAsync(roleName);
                if (role is null)
                {
                    role = new AspNetRoles
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    };
                    await roleRepo.AddAsync(role);
                    await roleRepo.SaveChangesAsync();
                    logger.LogInformation("Bootstrap: created missing role {Role}.", roleName);
                }

                if (roleName == "SuperAdmin")
                {
                    superAdminRole = role;
                }
                else if (roleName == "Admin")
                {
                    adminRole = role;
                }
            }

            return (superAdminRole!, adminRole!);
        }
}