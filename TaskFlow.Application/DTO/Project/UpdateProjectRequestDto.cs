namespace TaskFlow.Application.DTO.Project;

public sealed class UpdateProjectRequestDto
{
    public Guid ProjectId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }
}
