using ApiContracts;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserDto> AddUserAsync(CreateUserDto request);
    Task UpdateUserAsync(int id, UpdateUserDto request);
    Task<IReadOnlyList<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
}