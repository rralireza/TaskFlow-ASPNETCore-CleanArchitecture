using TaskFlow.Domain.Enums;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid ProjectId { get; set; }

    public virtual Project? Project { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public DateTime InsertDate { get; set; }

    public Guid InsertUser { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual User? InsertUserDetails { get; set; }

    public virtual ICollection<Comment>? Comments { get; set; }
}
