using TaskFlow.Application.DTO.TaskItem;

namespace TaskFlow.Application.Intefaces.Services.TaskItem;

public interface ITaskItemAdderService
{
    Task<TaskItemResponseDto> AddTaskItem(AddTaskItemRequestDto request);
}
