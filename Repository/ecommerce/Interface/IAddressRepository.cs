using WepApi.Helpers;
using WepApi.Models;

namespace WepApi.Repository.ecommerce.Interface;

public interface IAddressRepository
{
    Task<PageResult<Address>> GetAddressByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<List<Address>> GetAllAddressAsync(CancellationToken cancellationToken = default);
    Task<Address?> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Address> AddAddressAsync(Address request, CancellationToken cancellationToken = default);
    
    // Task<StoreTypes> UpdateStoreTypeAsync(StoreTypes request, CancellationToken cancellationToken = default);
    // Task<bool> DeleteStoreTypeAsync(int id, CancellationToken cancellationToken = default);
}