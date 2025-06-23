namespace TaskFlow.Application.Intefaces.Services.Users;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}
