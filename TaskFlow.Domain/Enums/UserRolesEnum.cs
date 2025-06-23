using System.ComponentModel;

namespace TaskFlow.Domain.Enums;

public enum UserRolesEnum : byte
{
    [Description("User")]
    User = 1,
    [Description("Admin")]
    Admin = 2
}
