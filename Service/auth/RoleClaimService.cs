using AutoMapper;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class RoleClaimService : IRoleClaimService
{
    private readonly IRoleClaimRepository _repository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleClaimService(IRoleClaimRepository repository, IRoleRepository roleRepository, IMapper mapper)
    {
        _repository = repository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }
    public async Task<List<RoleClaimResponse>> GetByRoleIdAsync(string roleId)
    {
        var claims = await _repository.GetByRoleIdAsync(roleId);
        return _mapper.Map<List<RoleClaimResponse>>(claims);
    }

    public async Task<RoleClaimResponse> AddAsync(RoleClaimRequest request)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
            throw new NotFoundException("Role Not Found");

        var claim = _mapper.Map<AspNetRoleClaims>(request);
        await _repository.AddAsync(claim);
        await _repository.SaveChangesAsync();
        return _mapper.Map<RoleClaimResponse>(claim);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var claim = await _repository.GetByIdAsync(id);
        if (claim == null)
            throw new NotFoundException("Role claim not found");

        _repository.Delete(claim);
        await _repository.SaveChangesAsync();
        return true;
    }
    
}