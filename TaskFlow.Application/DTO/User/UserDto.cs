namespace TaskFlow.Application.DTO.User;

public sealed class UserDto
{
    public Guid Id { get; set; }

    public string Fullname { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
}
