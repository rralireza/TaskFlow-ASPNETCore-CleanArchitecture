using TaskFlow.Application.DTO.Project;

namespace TaskFlow.Application.Intefaces.Services.Projects;

public interface IProjectAdderService
{
    Task<ProjectResponseDto> CreateProject(AddProjectRequestDto request);
}
