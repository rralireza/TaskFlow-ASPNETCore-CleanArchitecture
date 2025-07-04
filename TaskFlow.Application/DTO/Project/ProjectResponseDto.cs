namespace TaskFlow.Application.DTO.Project;

public sealed class ProjectResponseDto
{
    public Guid ProjectId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreateByUser { get; set; }

    public string CreateByUserRole { get; set; }
}
