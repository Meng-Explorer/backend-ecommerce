using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Helpers;
using WepApi.Models;
using WepApi.Repository.ecommerce.Interface;

namespace WepApi.Repository.ecommerce;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;
    
    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<Address>> GetAddressByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var data = await _context.Addresses
            .Include(a => a.User)
            .AsNoTracking() // សន្សំ RAM និង CPU 
            .OrderBy(a => a.Id) // តម្រៀបតាម Id សម្រាប់ការកាត់ទំព័រ
            // ប្រសិនបើ ToPagedListAsync របស់អ្នកអាចទទួលយក cancellationToken សូមបន្ថែមវាចូលទីនេះដែរ
            .ToPagedListAsync(page, pageSize); 
        return data;
    }

    public async Task<List<Address>> GetAllAddressAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .Include(a => a.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken); // បញ្ចូល Token
    }
    
    public async Task<Address?> GetAddressByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .Include(a => a.User!)
            .AsNoTracking() 
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken); // បញ្ចូល Token
    }

    public async Task<Address> AddAddressAsync(Address request, CancellationToken cancellationToken = default)
    {
        await _context.Addresses.AddAsync(request, cancellationToken); // បញ្ចូល Token
        await _context.SaveChangesAsync(cancellationToken);
        
        await _context.Entry(request)
            .Reference(a => a.User)
            .LoadAsync(cancellationToken);
        return request;
    }
}