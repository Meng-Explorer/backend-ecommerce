using WepApi.DTOs.ecommerce.Request;
using WepApi.DTOs.ecommerce.Response;
using WepApi.Helpers;

namespace WepApi.Service.ecommerce.Interface;

public interface IAddressService
{
    Task<PageResult<AddressResponse>> GetAddressByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<List<AddressResponse>> GetAllAddressAsync(CancellationToken cancellationToken = default);
    Task<AddressResponse?> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AddressResponse> AddAddressAsync(AddressRequest request, CancellationToken cancellationToken = default);
    
    // Task<StoreTypesResDto> UpdateStoreTypeAsync(int id, StoreTypesReqDto request, CancellationToken cancellationToken = default);
    // Task<bool> DeleteStoreTypeAsync(int id, CancellationToken cancellationToken = default);
}