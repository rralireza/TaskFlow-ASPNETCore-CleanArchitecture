namespace TaskFlow.Application.DTO.TaskItem;

public sealed class TaskItemResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime DeadLine { get; set; }
    public string Project { get; set; } = string.Empty;
}
