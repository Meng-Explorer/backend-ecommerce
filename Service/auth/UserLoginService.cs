using AutoMapper;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class UserLoginService : IUserLoginService
{
    private readonly IUserLoginRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserLoginService(IUserLoginRepository repository, IUserRepository userRepository, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<List<UserLoginResponse>> GetByUserIdAsync(string userId)
    {
        var logins = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<List<UserLoginResponse>>(logins);
    }

    public async Task<UserLoginResponse> AddAsync(UserLoginRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User Not Found");

        var existing = await _repository.GetAsync(request.LoginProvider, request.ProviderKey);
        if (existing != null)
            throw new BadRequestException("This external login is already linked to a user");

        var login = _mapper.Map<AspNetUserLogins>(request);
        await _repository.AddAsync(login);
        await _repository.SaveChangesAsync();
        return _mapper.Map<UserLoginResponse>(login);
    }

    public async Task<bool> RemoveAsync(string loginProvider, string providerKey)
    {
        var login = await _repository.GetAsync(loginProvider, providerKey);
        if (login == null)
            throw new NotFoundException("User login not found");

        _repository.Delete(login);
        await _repository.SaveChangesAsync();
        return true;
    }
    
    

}