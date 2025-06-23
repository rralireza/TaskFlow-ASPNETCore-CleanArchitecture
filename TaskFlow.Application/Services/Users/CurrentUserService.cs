using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskFlow.Application.Intefaces.Services.Users;

namespace TaskFlow.Application.Services.Users;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var userIdString = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdString, out var userId))
        {
            UserId = userId;
        }
        else
        {
            UserId = null;
        }
    }
}
