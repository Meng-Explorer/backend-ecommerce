using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WepApi.Data;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Repository.ecommerce;
using WepApi.Repository.ecommerce.Interface;
using WepApi.Service;
using WepApi.Service.ecommerce;
using WepApi.Service.ecommerce.Interface;
using WepApi.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

var conStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conStr))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured. Set it with " +
        "dotnet user-secrets set ConnectionStrings:DefaultConnection <value>, " +
        "or the ConnectionStrings__DefaultDatabase environment variable.");
}

var jwtSecret = builder.Configuration["JWT:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException(
        "JWT:Secret is missing or shorter than 32 bytes, which is unsafe for HMAC-SHA256. " +
        "Generate one with 'openssl rand -base64 48' and set it with " +
        "dotnet user-secrets set JWT:Secret <value>, or the JWT__Secret environment variable.");
}

var smtpServer = builder.Configuration["EmailSettings:SmtpServer"];
var smtpPort = builder.Configuration["EmailSettings:Port"];
var smtpUser = builder.Configuration["EmailSettings:SenderEmail"];
var smtpPassword = builder.Configuration["EmailSettings:SenderPassword"];

if (builder.Environment.IsProduction() &&
    (string.IsNullOrWhiteSpace(smtpServer)
        || string.IsNullOrWhiteSpace(smtpUser)
        || string.IsNullOrWhiteSpace(smtpPassword)
        || !int.TryParse(smtpPort, out _)))
{
    throw new InvalidOperationException(
        "EmailSettings are not configured for production. Set EmailSettings__SmtpServer, " +
        "EmailSettings__Port, EmailSettings__SenderEmail, and EmailSettings__SenderPassword.");
}

// Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(conStr);
});

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{

    options.SuppressMapClientErrors = true;

    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Invalid value."
                        : error.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(new
        {
            success = false,
            statusCode = StatusCodes.Status400BadRequest,
            message = "One or more validation errors occurred.",
            errors
        });
    };
});

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperConfiguration>());

// OTP
builder.Services.AddMemoryCache();

// Add Scoped Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserClaimRepository, UserClaimRepository>();
builder.Services.AddScoped<IUserLoginRepository, UserLoginRepository>();
builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
builder.Services.AddScoped<IRoleClaimRepository, RoleClaimRepository>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();




builder.Services.AddHttpClient();




// Add Scoped Services
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<PasswordHasher<AspNetUsers>>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserClaimService, UserClaimService>();
builder.Services.AddScoped<IUserLoginService, UserLoginService>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();
builder.Services.AddScoped<IRoleClaimService, RoleClaimService>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebApi",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter only your JWT token (no need to type Bearer)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policyBuilder =>
    {
        if (allowedOrigins.Contains("*"))
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
        else
        {
            // With no origins configured this allows nothing rather than everything.
            policyBuilder
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    });
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Strict window for authentication attempts.
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    // General ceiling for everything else.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Configure Authentication
var authBuilder = builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddCookie(IdentityConstants.ExternalScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
})
.AddJwtBearer(options =>
{
    // Require HTTPS for token metadata everywhere except local development.
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

        // Pin the algorithm so a token cannot arrive signed with something weaker.
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },

        // Default skew is 5 minutes, which keeps expired tokens usable too long.
        ClockSkew = TimeSpan.FromSeconds(30),

        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    // Accept token with or without the "Bearer " prefix in Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    context.Token = authHeader.Trim();
                }
            }
            return Task.CompletedTask;
        }
    };
});

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SignInScheme = IdentityConstants.ExternalScheme;
    });
}

var githubClientId = builder.Configuration["Authentication:Github:ClientId"];
var githubClientSecret = builder.Configuration["Authentication:Github:ClientSecret"];
if (!string.IsNullOrWhiteSpace(githubClientId) && !string.IsNullOrWhiteSpace(githubClientSecret))
{
    authBuilder.AddGitHub(options =>
    {
        options.ClientId = githubClientId;
        options.ClientSecret = githubClientSecret;
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.Scope.Add("user:email");
    });
}


builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
// {
//    var superAdminRoles = builder.Configuration
//                              .GetSection("Authorization:SuperAdminRoles").Get<string[]>() 
//                          ?? new[] { "SuperAdmin" };
//
//    var adminRoles = (builder.Configuration
//                          .GetSection("Authorization:AdminRoles").Get<string[]>() 
//                      ?? new[] { "Admin" })
//        .Concat(superAdminRoles).Distinct().ToArray();

//    var managerRoles = (builder.Configuration
//                            .GetSection("Authorization:ManagerRoles").Get<string[]>() 
//                        ?? new[] { "Manager" })
//        .Concat(adminRoles).Distinct().ToArray(); 
//
//    var customerRoles = builder.Configuration
//                            .GetSection("Authorization:CustomerRoles").Get<string[]>() 
//                        ?? new[] { "User" };
//
//    var allAuthenticatedRoles = managerRoles.Concat(customerRoles).Distinct().ToArray();
//
//    options.AddPolicy("SuperAdminOnly", policy =>
//        policy.RequireRole(superAdminRoles));
//    
//    options.AddPolicy("CanDeleteCatalog", policy =>
//        policy.RequireRole(adminRoles));
//    
//    options.AddPolicy("CanManageCatalog", policy =>
//        policy.RequireRole(managerRoles));
    
//    options.AddPolicy("CanAccessDashboard", policy =>
//        policy.RequireRole(managerRoles));
    
//   options.AddPolicy("CustomerAccess", policy =>
//        policy.RequireRole(allAuthenticatedRoles));
//});

var app = builder.Build();

// Guarantees the roles the policies above reference exist, and that somebody can
// administer the system. Idempotent; creates an account only when configured.
await IdentitySeeder.SeedAsync(app.Services);

app.UseMiddleware<WepApi.Middlewares.ExceptionMiddleware>();

// 2. Security Protocol
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Baseline security response headers.
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "no-referrer";
    await next();
});

// 3. Routing
app.UseRouting();

// 4. CORS
app.UseCors("Default");

// 5. Rate limiting
app.UseRateLimiter();

// Serve static files from wwwroot (uploads/images, etc.)
app.UseStaticFiles();

var swaggerEnabled = app.Configuration.GetValue<bool?>("Swagger:Enabled")
                     ?? app.Environment.IsDevelopment();
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
        options.RoutePrefix = string.Empty;
    });
}


app.Use(async (context, next) =>
{
    await next();

    var status = context.Response.StatusCode;
    if ((status == (int)HttpStatusCode.Unauthorized
         || status == (int)HttpStatusCode.Forbidden
         || status == (int)HttpStatusCode.UnsupportedMediaType)
        && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            Success = false,
            StatusCode = status,
            Message = status switch
            {
                (int)HttpStatusCode.Unauthorized => "Authentication required.",
                (int)HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
                _ => $"Unsupported Content-Type '{context.Request.ContentType ?? "none"}' for this endpoint."
            },
            Data = new { }
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        await context.Response.WriteAsync(json);
    }
});

// 7. Identity verification
app.UseAuthentication();
app.UseAuthorization();

// 8. Map Endpoints
app.MapControllers();

app.Run();
