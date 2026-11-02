using AutoMapper;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class UserClaimService : IUserClaimService
{
    private readonly IUserClaimRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserClaimService(IUserClaimRepository repository, IUserRepository userRepository, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<List<UserClaimResponse>> GetByUserIdAsync(string userId)
    {
        var claims = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<List<UserClaimResponse>>(claims);
    }

    public async Task<UserClaimResponse> AddAsync(UserClaimRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User Not Found");

        var claim = _mapper.Map<AspNetUserClaims>(request);
        await _repository.AddAsync(claim);
        await _repository.SaveChangesAsync();
        return _mapper.Map<UserClaimResponse>(claim);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var claim = await _repository.GetByIdAsync(id);
        if (claim == null)
            throw new NotFoundException("User claim not found");

        _repository.Delete(claim);
        await _repository.SaveChangesAsync();
        return true;
    }
    
    
}