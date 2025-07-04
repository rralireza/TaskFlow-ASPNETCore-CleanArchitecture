namespace TaskFlow.Application.DTO.TaskItem;

public sealed class AddTaskItemRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public byte Status { get; set; }
    public byte Priority { get; set; }
    public DateTime DeadLine { get; set; } = DateTime.Now;
    public Guid ProjectId { get; set; }
    public Guid AssignedToUserId { get; set; }
}
