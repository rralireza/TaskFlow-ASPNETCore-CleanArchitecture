using FluentValidation;
using Moq;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Projects;

public class ProjectAdderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IValidator<AddProjectRequestDto>> _validatorMock = new();
    private readonly Mock<IProjectPoliciesService> _projectPoliciesMock = new();

    private readonly ProjectAdderService _service;

    public ProjectAdderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _validatorMock = new Mock<IValidator<AddProjectRequestDto>>();
        _projectPoliciesMock = new Mock<IProjectPoliciesService>();

        _service = new ProjectAdderService(
            _unitOfWorkMock.Object,
            _validatorMock.Object,
            _currentUserServiceMock.Object,
            _projectPoliciesMock.Object);
    }

    [Fact]
    public async Task CreateProject_ShoulCreateProject_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        AddProjectRequestDto request = new()
        {
            Title = "Test Project",
            Description = "Test Description"
        };

        User user = new()
        {
            Id = userId,
            Email = "test@example.com",
            Role = (byte)UserRolesEnum.User
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _validatorMock
            .Setup(x => x.Validate(It.IsAny<AddProjectRequestDto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        _unitOfWorkMock.Setup(x => x.Users.GetByIdAsync(userId)).ReturnsAsync(user);

        _projectPoliciesMock
            .Setup(x => x.DuplicateTitle(userId, request.Title))
            .Returns(false);

        _projectPoliciesMock
            .Setup(x => x.CanCreateProject(userId, (byte)UserRolesEnum.User, 4))
            .Returns(true);

        Project? project = null;

        _unitOfWorkMock.Setup(x => x.Projects.AddAsync(It.IsAny<Project>()))
            .Callback<Project>(p => project = p)
            .Returns(Task.CompletedTask); // Fix: Changed Task.CompletedTask to Task.FromResult(0)

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .ReturnsAsync(1); // Fix: Ensure SaveAsync returns Task<int>

        //Act
        var result = await _service.CreateProject(request);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal("test@example.com", result.CreateByUser);
        Assert.Equal("User", result.CreateByUserRole);

        Assert.NotNull(project);
        Assert.Equal(userId, project.CreatedByUserId);
    }

    [Fact]
    public async Task CreateProject_ShouldThrowValidationException_WhenValidationFails()
    {
        //Arrange
        AddProjectRequestDto request = new()
        {
            Title = "",
            Description = "Test Description"
        };

        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Title", "Title is required")
        };

        _validatorMock
            .Setup(x => x.Validate(request))
            .Returns(new FluentValidation.Results.ValidationResult(validationFailures));

        //Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
        {
            await _service.CreateProject(request);
        });
    }

    [Fact]
    public async Task CreateProject_ShouldThrowValidationException_WhenTitleIsAlreadyExists()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        AddProjectRequestDto requrest = new()
        {
            Title = "Duplicate Project",
            Description = "Test Description"
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _validatorMock
            .Setup(x => x.Validate(requrest))
            .Returns(new FluentValidation.Results.ValidationResult());

        _unitOfWorkMock
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(new User
            {
                Id = userId,
                Role = (byte)UserRolesEnum.User,
                Fullname = "Test User",
                Email = "test@exmaple.com"
            });

        _projectPoliciesMock
            .Setup(x => x.DuplicateTitle(userId, requrest.Title))
            .Returns(true);

        //Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _service.CreateProject(requrest));
    }

    [Fact]
    public async Task CreateProject_ShouldThrowValidationException_WhenUserReachedProjectLimit()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        AddProjectRequestDto request = new()
        {
            Title = "Test Title",
            Description = "Test Description"
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _validatorMock
            .Setup(x => x.Validate(request))
            .Returns(new FluentValidation.Results.ValidationResult());

        _unitOfWorkMock
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(new User
            {
                Id = userId,
                Role = (byte)UserRolesEnum.User,
                Fullname = "Test User",
                Email = "test@example.com"
            });

        _projectPoliciesMock
            .Setup(x => x.CanCreateProject(userId, (byte)UserRolesEnum.User, 4))
            .Returns(false);

        //Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _service.CreateProject(request));
    }
}
