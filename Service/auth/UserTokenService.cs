using AutoMapper;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class UserTokenService : IUserTokenService
{

    private readonly IUserTokenRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserTokenService(IUserTokenRepository repository, IUserRepository userRepository, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<List<UserTokenResponse>> GetByUserIdAsync(string userId)
    {
        var tokens = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<List<UserTokenResponse>>(tokens);
    }

    public async Task<UserTokenResponse> SetTokenAsync(UserTokenRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User Not Found");

        var existing = await _repository.GetAsync(request.UserId, request.LoginProvider, request.Name);
        if (existing != null)
        {
            existing.Value = request.Value;
            await _repository.SaveChangesAsync();
            return _mapper.Map<UserTokenResponse>(existing);
        }

        var token = _mapper.Map<AspNetUserTokens>(request);
        await _repository.AddAsync(token);
        await _repository.SaveChangesAsync();
        return _mapper.Map<UserTokenResponse>(token);
    }

    public async Task<bool> RemoveAsync(string userId, string loginProvider, string name)
    {
        var token = await _repository.GetAsync(userId, loginProvider, name);
        if (token == null)
            throw new NotFoundException("User token not found");

        _repository.Delete(token);
        await _repository.SaveChangesAsync();
        return true;
    }


}