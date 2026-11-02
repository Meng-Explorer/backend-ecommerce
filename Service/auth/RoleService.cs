using AutoMapper;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;
    public RoleService(IRoleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<List<RoleResponse>> GetAllAsync()
        {
            var roles = await _repository.GetAllAsync();
            return _mapper.Map<List<RoleResponse>>(roles);
        }

        public async Task<RoleResponse?> GetByIdAsync(string Id)
        {
            var role = await _repository.GetByIdAsync(Id);
            if (role == null)
                throw new NotFoundException("Role Not Found");
            return _mapper.Map<RoleResponse>(role);
        }

        public async Task<RoleResponse> CreateAsync(RoleRequest request)
        {
            var name = request.Name.Trim();
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Role name is Required");
            }
            var exists = await _repository.ExistsByNameAsync(name);
            if (exists)
            {
                throw new InvalidOperationException("Role name already exists");
            }
            var role = _mapper.Map<AspNetRoles>(request);
            await _repository.AddAsync(role);
            await _repository.SaveChangesAsync();
            return _mapper.Map<RoleResponse>(role);
        }

        public async Task<RoleResponse?> UpdateAsync(string Id, RoleRequest request)
        {
            var role = await _repository.GetByIdAsync(Id);
            if (role == null)
                throw new NotFoundException("Role Not Found");
            var name = request.Name.Trim();
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Role name is Required");
            }
            var exists = await _repository.ExistsByNameAsync(name, Id);
            if (exists)
            {
                throw new InvalidOperationException("Role name already exists");
            }
            _mapper.Map(request, role);

            await _repository.Update(role);
            await _repository.SaveChangesAsync();
            return _mapper.Map<RoleResponse>(role);

        } 
        public async Task<bool> DeleteAsync(string Id)
        {
            var role = await _repository.GetByIdAsync(Id);
            if (role == null)
                throw new NotFoundException("Role not Found");
            var hasUsers = await _repository.HasUsersAsync(Id);
            if (hasUsers)
            {
                throw new InvalidOperationException("Cannot delete this role because it is assigned to one or more users.");
            } 
            _repository.Delete(role);
            await _repository.SaveChangesAsync();
            return true;
        }
    
}