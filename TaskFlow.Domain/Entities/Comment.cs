namespace TaskFlow.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Guid TaskItemId { get; set; }

    public virtual TaskItem? TaskItem { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
}
