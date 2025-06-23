namespace TaskFlow.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual ICollection<TaskItem>? Tasks { get; set; }
}
