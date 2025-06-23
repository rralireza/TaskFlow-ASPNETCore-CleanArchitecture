using TaskFlow.Application.DTO.User;

namespace TaskFlow.Application.Intefaces.Services.Users;

public interface IUserAdderService
{
    Task<UserDto> RegisterUserAsync(CreateUserDto request);

    Task<UserDto> AddUserAsync(CreateUserDto request);
    Task<AuthResultDto> LoginAsync(LoginUserDto request);

    bool IsEmailExists(string email);
}
