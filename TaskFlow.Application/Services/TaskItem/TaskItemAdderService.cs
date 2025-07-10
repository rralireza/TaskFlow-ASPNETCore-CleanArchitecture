using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Application.Intefaces.Services.TaskItem;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.TaskItem;

public sealed class TaskItemAdderService : ITaskItemAdderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddTaskItemRequestDto> _validator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TaskItemAdderService> _logger;

    public TaskItemAdderService(IUnitOfWork unitOfWork, IValidator<AddTaskItemRequestDto> validator, ICurrentUserService currentUserService, ILogger<TaskItemAdderService> logger)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    private bool DuplicateTask(string title, Guid projectId)
    {
        return _unitOfWork
            .Tasks
            .WhereQueryable(x => x.Title == title && x.ProjectId == projectId)
            .Any();
    }

    public async Task<TaskItemResponseDto> AddTaskItem(AddTaskItemRequestDto request)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Please login first then try again!");

        var currentUser = await _unitOfWork
            .Users
            .GetByIdAsync(currentUserId) ?? throw new Exception("User not found!");

        var project = await _unitOfWork
            .Projects
            .GetByIdAsync(request.ProjectId) ?? throw new Exception("Project not found!");

        bool isAdmin = currentUser.Role == (byte)UserRolesEnum.Admin;
        bool isOwner = project.CreatedByUserId == currentUserId;

        if (!isAdmin && !isOwner)
            throw new UnauthorizedAccessException("You don't have permission to add tasks to this project!");

        if (DuplicateTask(request.Title, request.ProjectId))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Title", $"{request.Title} is already exists!")
            });
        }

        if (request.AssignedToUserId != Guid.Empty && !isAdmin && !isOwner)
            throw new UnauthorizedAccessException("You can't assign this task to other users");

        var task = new Domain.Entities.TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = (Domain.Enums.TaskStatus)request.Status,
            Priority = (TaskPriority)request.Priority,
            Deadline = request.DeadLine,
            CreatedAt = DateTime.Now,
            ProjectId = request.ProjectId,
            AssignedToUserId = request.AssignedToUserId,
            InsertUser = currentUserId,
        };

        await _unitOfWork
            .Tasks
            .AddAsync(task);

        await _unitOfWork.SaveAsync();

        return new TaskItemResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DeadLine = task.Deadline,
            Project = project.Title
        };
    }
}
