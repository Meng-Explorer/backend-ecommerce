using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WepApi.Models;

namespace WepApi.Data;

public class ApplicationDbContext : IdentityDbContext<
    AspNetUsers,
    AspNetRoles,
    string,
    AspNetUserClaims,
    AspNetUserRoles,
    AspNetUserLogins,
    AspNetRoleClaims,
    AspNetUserTokens>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Direct access to Identity Sets using the Model class names
    public DbSet<AspNetUsers> AspNetUsers => Users;
    public DbSet<AspNetRoles> AspNetRoles => Roles;
    public DbSet<AspNetUserRoles> AspNetUserRoles => UserRoles;
    public DbSet<AspNetRoleClaims> AspNetRoleClaims => RoleClaims;
    public DbSet<AspNetUserClaims> AspNetUserClaims => UserClaims;
    public DbSet<AspNetUserLogins> AspNetUserLogins => UserLogins;
    public DbSet<AspNetUserTokens> AspNetUserTokens => UserTokens;

    // eCommerce Entity Sets
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<Categories> Categories { get; set; }
    public virtual DbSet<Products> Products { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<Carts> Carts { get; set; }
    public virtual DbSet<CartItems> CartItems { get; set; }
    public virtual DbSet<Orders> Orders { get; set; }
    public virtual DbSet<OrderItems> OrderItems { get; set; }
    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public virtual DbSet<Payments> Payments { get; set; }
    public virtual DbSet<Coupons> Coupons { get; set; }
    public virtual DbSet<CouponUsages> CouponUsages { get; set; }
    public virtual DbSet<Reviews> Reviews { get; set; }
    public virtual DbSet<Wishlists> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        
        builder.Entity<AspNetUserRoles>(entity =>
        {
            entity.HasOne(d => d.Role)
                .WithMany(p => p.AspNetUserRoles)
                .HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.User)
                .WithMany(p => p.AspNetUserRoles)
                .HasForeignKey(d => d.UserId);
        });

        builder.Entity<AspNetRoleClaims>(entity =>
        {
            entity.HasOne(d => d.Role)
                .WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId);
        });

        builder.Entity<AspNetUserClaims>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId);
        });

        builder.Entity<AspNetUserLogins>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId);
        });

        builder.Entity<AspNetUserTokens>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId);
        });

        // Address -> AspNetUsers
        builder.Entity<Address>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Categories -> Products
        builder.Entity<Categories>(entity =>
        {
            entity.HasMany(d => d.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Products
        builder.Entity<Products>(entity =>
        {
            entity.Property(e => e.Price).HasPrecision(18, 2);

            entity.HasMany(d => d.ProductImages)
                .WithOne(p => p.Products)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.CartItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.OrderItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Reviews)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Wishlists)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Carts & CartItems
        builder.Entity<Carts>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.CartItems)
                .WithOne(p => p.Cart)
                .HasForeignKey(p => p.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Orders
        builder.Entity<Orders>(entity =>
        {
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.ShippingFee).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Avoid multiple cascade paths in SQL Server
            entity.HasOne(d => d.Address)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.OrderItems)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.OrderStatusHistories)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.CouponUsages)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItems
        builder.Entity<OrderItems>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.LineTotal).HasPrecision(18, 2);
        });

        // Payments
        builder.Entity<Payments>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // Coupons & CouponUsages
        builder.Entity<Coupons>(entity =>
        {
            entity.Property(e => e.DiscountValue).HasPrecision(18, 2);
            entity.Property(e => e.MinOrderAmount).HasPrecision(18, 2);

            entity.HasMany(d => d.CouponUsages)
                .WithOne(p => p.Coupon)
                .HasForeignKey(p => p.CouponId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CouponUsages>(entity =>
        {
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);

            entity.HasOne(d => d.User)
                .WithMany(p => p.CouponUsages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Reviews
        builder.Entity<Reviews>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Wishlists
        builder.Entity<Wishlists>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
