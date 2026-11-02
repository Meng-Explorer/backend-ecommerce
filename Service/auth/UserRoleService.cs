using AutoMapper;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserRoleService(IUserRoleRepository repository, IUserRepository userRepository,
        IRoleRepository roleRepository, IConfiguration configuration, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public async Task<UserRoleResponse?> GetUserRolesAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User Not Found");
            var roles = await _repository.GetUserRolesAsync(userId);
            return new UserRoleResponse
            {
                UserId = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Roles = _mapper.Map<List<RoleResponse>>(roles)
            };
        }

        public async Task<bool> AssignRoleAsync(string userId,string roleId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User Not Found.");
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new NotFoundException("Role Not Found");

            ValidateSuperAdminAssignment(user, role.Name);

            var exists = await _repository.HashRoleAsync(userId, roleId);
            if (exists)
                throw new BadRequestException("User already has this role");
            var userRole = new AspNetUserRoles
            {
                UserId = userId,
                RoleId = roleId,
            };

            await _repository.AddUserRoleAsync(userRole);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignRolesAsync(string userId,List<string> roleIds)
        {
            var user = await _userRepository.GetUserByIdAsync (userId);
            if (user == null)
                throw new NotFoundException("User Not Found");
            if (roleIds == null || roleIds.Count == 0)
                throw new NotFoundException("At least one role is Required");
            roleIds = roleIds.Distinct().ToList();
            var newRoles = new List<AspNetUserRoles>();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync (roleId);
                if (role == null)
                    throw new NotFoundException($"Role ID {roleId} Not Found");

                ValidateSuperAdminAssignment(user, role.Name);

                var exists = await _repository.HashRoleAsync (userId, roleId);
                if (!exists)
                {
                    newRoles.Add(new AspNetUserRoles { UserId = userId, RoleId = roleId });
                }
            }
            if (newRoles.Count == 0)
                throw new BadRequestException("No new roles to assign (Roles might already exist).");
            await _repository.AddUserRolesAsync(newRoles);
            await _repository.SaveChangesAsync();
            return true;
        }

        private void ValidateSuperAdminAssignment(AspNetUsers user, string? roleName)
        {
            if (string.Equals(roleName, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                var allowedUsername = _configuration["Bootstrap:AdminUsername"]?.Trim().ToLowerInvariant();
                var allowedEmail = _configuration["Bootstrap:AdminEmail"]?.Trim().ToLowerInvariant();

                bool isDesignated = (!string.IsNullOrEmpty(allowedUsername) && string.Equals(user.UserName, allowedUsername, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(allowedEmail) && string.Equals(user.Email, allowedEmail, StringComparison.OrdinalIgnoreCase));

                if (!isDesignated)
                {
                    throw new BadRequestException("Only the designated system administrator can hold the SuperAdmin role.");
                }
            }
        }

        public async Task<bool> RemoveRoleAsync(string userId, string roleId)
        {
            var userRole = await _repository.GetUserRoleAsync(userId, roleId);
            if (userRole == null)
                throw new BadRequestException("User role not found");
            _repository.RemoveUserRole(userRole);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRolesAsync(string userId,List<string> roleIds)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User Not Found");
            if (roleIds == null || roleIds.Count == 0)
                throw new BadRequestException("At least one role is required");
            roleIds = roleIds.Distinct().ToList();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    throw new NotFoundException($"Role ID {roleId} not found");
            }
            var existingRoles = await _repository.GetUserRoleEntitiesASync(userId);
            var rolesToRemove = existingRoles
                .Where(userRole => roleIds.Contains(userRole.RoleId))
                .ToList();
            if (rolesToRemove.Count == 0)
                throw new BadRequestException("User does not have any of the specified roles");
            _repository.RemoveUserRoles(rolesToRemove);
            await _repository.SaveChangesAsync();
            return true;
        }
    

}