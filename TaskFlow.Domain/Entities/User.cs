namespace TaskFlow.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Fullname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public byte Role { get; set; }

    public virtual ICollection<Project>? ProjectsCreated { get; set; }

    public virtual ICollection<TaskItem>? AssignedTasks { get; set; }

    public virtual ICollection<Comment>? Comments { get; set; }
}
