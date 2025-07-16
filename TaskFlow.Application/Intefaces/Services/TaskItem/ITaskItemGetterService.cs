using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.DTO.Pagination;
using TaskFlow.Application.DTO.TaskItem;

namespace TaskFlow.Application.Intefaces.Services.TaskItem;

public interface ITaskItemGetterService
{
    Task<PagingResult<TaskItemResponseDto>> GetAllTasksForCurrentUser(TaskFilterDto filter);
}
