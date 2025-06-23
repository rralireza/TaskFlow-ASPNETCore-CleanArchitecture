using TaskFlow.Application.DTO.Project;

namespace TaskFlow.Application.Intefaces.Services.Projects;

public interface IProjectUpdaterService
{
    Task<ProjectResponseDto> UpdateProject(UpdateProjectRequestDto request);
}
