using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.DTO.Pagination;
using TaskFlow.Application.DTO.Project;

namespace TaskFlow.Application.Intefaces.Services.Projects;

public interface IProjectGetterService
{
    Task<PagingResult<ProjectResponseDto>> GetAllProjectsForCurrentUser(ProjectFilterDto filter);
}
