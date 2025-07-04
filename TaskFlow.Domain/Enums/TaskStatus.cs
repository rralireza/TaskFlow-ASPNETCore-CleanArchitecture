using System.ComponentModel;

namespace TaskFlow.Domain.Enums;

public enum TaskStatus : byte
{
    [Description("To Do")]
    ToDo = 1,

    [Description("In Progress")]
    InProgress = 2,

    [Description("Done")]
    Done = 3
}
