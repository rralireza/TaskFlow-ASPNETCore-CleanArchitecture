using FluentValidation;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Application.Intefaces.Services.TaskItem;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;

namespace TaskFlow.Application.Services.TaskItem;

public sealed class TaskItemAdderService : ITaskItemAdderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddTaskItemRequestDto> _validator;
    private readonly ICurrentUserService _currentUserService;

    public TaskItemAdderService(IUnitOfWork unitOfWork, IValidator<AddTaskItemRequestDto> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public Task<TaskItemResponseDto> AddTaskItem(AddTaskItemRequestDto request)
    {
        throw new NotImplementedException();
    }
}
