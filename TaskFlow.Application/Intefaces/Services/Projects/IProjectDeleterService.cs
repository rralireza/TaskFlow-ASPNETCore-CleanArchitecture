using TaskFlow.Application.DTO.Project;

namespace TaskFlow.Application.Intefaces.Services.Projects;

public interface IProjectDeleterService
{
    Task<ProjectResponseDto> DeleteProject(Guid projectId);
}
