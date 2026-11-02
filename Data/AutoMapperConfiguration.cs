using AutoMapper;
using WepApi.DTOs.ecommerce.Request;
using WepApi.DTOs.ecommerce.Response;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Models;
using OrderStatusHistory = WepApi.Models.OrderStatusHistory;

namespace WepApi.Data;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        // Auth
        CreateMap<RegisterRequest, AspNetUsers>()
            .ForMember(destination => destination.PasswordHash, options => options.Ignore());
        CreateMap<AspNetUsers, UserResponse>().ReverseMap();

        CreateMap<AspNetRoles, RoleRequest>().ReverseMap();
        CreateMap<AspNetRoles, RoleResponse>().ReverseMap();

        CreateMap<AspNetUserRoles, UserRoleRequest>().ReverseMap();
        CreateMap<AspNetUserRoles, UserRoleResponse>().ReverseMap();

        CreateMap<AspNetUserClaims, UserClaimRequest>().ReverseMap();
        CreateMap<AspNetUserClaims, UserClaimResponse>().ReverseMap();

        CreateMap<AspNetUserLogins, UserLoginRequest>().ReverseMap();
        CreateMap<AspNetUserLogins, UserLoginResponse>().ReverseMap();

        CreateMap<AspNetUserTokens, UserTokenRequest>().ReverseMap();
        CreateMap<AspNetUserTokens, UserTokenResponse>().ReverseMap();

        CreateMap<AspNetRoleClaims, RoleClaimRequest>().ReverseMap();
        CreateMap<AspNetRoleClaims, RoleClaimResponse>().ReverseMap();
        
        // Ecommerce
        CreateMap<Address,AddressRequest>().ReverseMap();
        CreateMap<Address,AddressResponse>().ReverseMap();
        
        CreateMap<CartItems,CartItemRequest>().ReverseMap();
        CreateMap<CartItems,CartItemResponse>().ReverseMap();
        
        CreateMap<Carts,CartRequest>().ReverseMap();
        CreateMap<Carts,CartResponse>().ReverseMap();
        
        CreateMap<Categories,CategoriesRequest>().ReverseMap(); 
        CreateMap<Categories,CategoriesResponse>().ReverseMap();
        
        CreateMap<Coupons,CouponRequest>().ReverseMap();
        CreateMap<Coupons,CouponResponse>().ReverseMap();
        
        CreateMap<CouponUsages,CouponUsageRequest>().ReverseMap();
        CreateMap<CouponUsages,CouponUsageResponse>().ReverseMap();
        
        CreateMap<OrderItems,OrderItemRequest>().ReverseMap();
        CreateMap<OrderItems,OrderItemResponse>().ReverseMap();
        
        CreateMap<Orders,OrderRequest>().ReverseMap();
        CreateMap<Orders,OrderResponse>().ReverseMap();
        
        CreateMap<OrderStatusHistory,OrderStatusHistoryRequest>().ReverseMap();
        CreateMap<OrderStatusHistory,OrderStatusHistoryResponse>().ReverseMap();
        
        CreateMap<Payments,PaymentRequest>().ReverseMap();
        CreateMap<Payments,PaymentResponse>().ReverseMap();
        
        CreateMap<ProductImage,ProductImageRequest>().ReverseMap();
        CreateMap<ProductImage,ProductImageResponse>().ReverseMap();
        
        CreateMap<Products,ProductRequest>().ReverseMap();
        CreateMap<Products,ProductResponse>().ReverseMap();
        
        CreateMap<Reviews,ReviewRequest>().ReverseMap();
        CreateMap<Reviews,ReviewResponse>().ReverseMap();
        
        CreateMap<Wishlists,WishlistRequest>().ReverseMap();
        CreateMap<Wishlists,WishlistResponse>().ReverseMap();
        
        
    }
}