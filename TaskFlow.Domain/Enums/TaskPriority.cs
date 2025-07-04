using System.ComponentModel;

namespace TaskFlow.Domain.Enums;

public enum TaskPriority : byte
{
    [Description("Low")]
    Low = 1,

    [Description("Medium")]
    Medium = 2,

    [Description("High")]
    High = 3
}
