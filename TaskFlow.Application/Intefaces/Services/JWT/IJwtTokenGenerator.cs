using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Intefaces.Services.JWT;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
