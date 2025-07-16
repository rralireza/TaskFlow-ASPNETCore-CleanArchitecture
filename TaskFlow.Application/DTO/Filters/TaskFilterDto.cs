namespace TaskFlow.Application.DTO.Filters;

public sealed class TaskFilterDto : PaginationFilterDto
{
    public DateTime? DeadLine { get; set; }
}
