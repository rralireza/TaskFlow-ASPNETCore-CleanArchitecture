using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Application.Services.TaskItem;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Tests.Tasks;

public class TaskAdderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IValidator<AddTaskItemRequestDto>> _validator = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<ILogger<TaskItemAdderService>> _logger = new();
    private readonly TaskItemAdderService _service;

    public TaskAdderServiceTests()
    {
        _service = new TaskItemAdderService(
            _unitOfWork.Object,
            _validator.Object,
            _currentUserService.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task AddTask_ShouldCreateTask_WhenDataIsValid()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();

        AddTaskItemRequestDto request = new()
        {
            Title = "Test Title",
            Description = "Test Description",
            Status = (byte)TaskStatus.ToDo,
            Priority = (byte)TaskPriority.Medium,
            DeadLine = DateTime.Now.AddDays(3),
            ProjectId = projectId,
            AssignedToUserId = Guid.Empty
        };

        User user = new()
        {
            Id = userId,
            Role = (byte)UserRolesEnum.Admin
        };

        Project project = new()
        {
            Id = projectId,
            CreatedByUserId = userId
        };

        _validator
            .Setup(x => x.Validate(It.IsAny<AddTaskItemRequestDto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        _currentUserService
            .Setup(X => X.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _unitOfWork
            .Setup(x => x.Projects.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        _unitOfWork
            .Setup(x => x.Tasks.WhereQueryable(It.IsAny<Expression<Func<TaskItem, bool>>>()))
            .Returns(Enumerable.Empty<TaskItem>().AsQueryable());

        TaskItem? insertedTask = null;

        _unitOfWork
            .Setup(x => x.Tasks.AddAsync(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(x => insertedTask = x)
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.SaveAsync())
            .ReturnsAsync(1);

        //Act
        var result = await _service.AddTaskItem(request);

        //Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(TaskStatus.ToDo.ToString(), result.Status);
        Assert.Equal(TaskPriority.Medium.ToString(), result.Priority);
        Assert.Equal(project.Title, result.Project);
        Assert.NotNull(insertedTask);
        Assert.Equal(userId, insertedTask.InsertUser);
    }

    [Fact]
    public async Task AddTask_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        //Arrange
        AddTaskItemRequestDto request = new()
        {
            Title = "Test Title",
            Description = "Test Description",
            Status = (byte)TaskStatus.ToDo,
            Priority = (byte)TaskPriority.Medium,
            DeadLine = DateTime.Now.AddDays(3),
            ProjectId = Guid.NewGuid(),
            AssignedToUserId = Guid.Empty
        };

        _validator
            .Setup(x => x.Validate(It.IsAny<AddTaskItemRequestDto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        //Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.AddTaskItem(request));
    }

    [Fact]
    public async Task AddTask_ShouldThrowException_WhenProjectNotFound()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        AddTaskItemRequestDto request = new()
        {
            Title = "Test Title",
            Description = "Test Description",
            DeadLine = DateTime.Now.AddDays(3),
            Status = (byte)TaskStatus.ToDo,
            Priority = (byte)TaskPriority.Medium,
            ProjectId = Guid.NewGuid(),
            AssignedToUserId = Guid.Empty
        };

        User user = new()
        {
            Id = userId,
            Role = (byte)UserRolesEnum.Admin
        };

        _validator
            .Setup(x => x.Validate(It.IsAny<AddTaskItemRequestDto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _unitOfWork
            .Setup(x => x.Projects.GetByIdAsync(request.ProjectId))
            .ReturnsAsync((Project?)null);

        //Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AddTaskItem(request));
    }

    [Fact]
    public async Task AddTask_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOWnerOrAdmin()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        Guid projectId = Guid.NewGuid();

        AddTaskItemRequestDto request = new()
        {
            Title = "Test Title",
            Description = "Test Description",
            Status = (byte)TaskStatus.ToDo,
            Priority = (byte)TaskPriority.Medium,
            DeadLine = DateTime.Now.AddDays(3),
            ProjectId = projectId,
            AssignedToUserId = Guid.Empty
        };

        User casualUser = new()
        {
            Id = userId,
            Role = (byte)UserRolesEnum.User
        };

        Project project = new()
        {
            Id = projectId,
            CreatedByUserId = Guid.NewGuid()
        };

        _validator
            .Setup(x => x.Validate(It.IsAny<AddTaskItemRequestDto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(casualUser);

        _unitOfWork
            .Setup(x => x.Projects.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        //Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.AddTaskItem(request));
    }
}
