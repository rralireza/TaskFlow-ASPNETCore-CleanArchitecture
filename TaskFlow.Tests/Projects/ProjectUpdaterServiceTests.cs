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

public class ProjectUpdaterServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IValidator<UpdateProjectRequestDto>> _validatorMock = new();
    private readonly Mock<IProjectPoliciesService> _projectPoliciesMock = new();

    private readonly ProjectUpdaterService _service;

    public ProjectUpdaterServiceTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<UpdateProjectRequestDto>>();
        _projectPoliciesMock = new Mock<IProjectPoliciesService>();

        _service = new ProjectUpdaterService(
            _unitOfWorkMock.Object,
            _currentUserServiceMock.Object,
            _validatorMock.Object,
            _projectPoliciesMock.Object);
    }

    [Fact]
    public async Task UpdateProject_ShouldUpdateProject_WhenDataIsValidAndUserIsOwner()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();

        Project existingProject = new()
        {
            Id = projectId,
            Title = "Old Project Title",
            Description = "Old Project Description",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId,
        };

        UpdateProjectRequestDto dto = new()
        {
            ProjectId = projectId,
            Title = "Updated Project Title",
            Description = "Updated Project Description"
        };


        User currentUser = new()
        {
            Id = userId,
            Email = "test@example.com",
            Role = (int)UserRolesEnum.Admin
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateProjectRequestDto>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _unitOfWorkMock
            .Setup(x => x.Projects.GetByIdAsync(projectId))
            .ReturnsAsync(existingProject);

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWorkMock
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(currentUser);

        _unitOfWorkMock
            .Setup(x => x.SaveAsync())
            .ReturnsAsync(1);

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _projectPoliciesMock
            .Setup(x => x.DuplicateTitle(userId, dto.Title))
            .Returns(false);

        //Act
        var result = await _service.UpdateProject(dto);

        //Await
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(currentUser.Email, result.CreateByUser);
        Assert.Equal(UserRolesEnum.Admin.ToString(), result.CreateByUserRole);

        _unitOfWorkMock.Verify(x => x.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_ShouldThrowArgumentNullException_WhenProjectDoesNotExists()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        UpdateProjectRequestDto dto = new()
        {
            ProjectId = Guid.NewGuid(),
            Title = "New Title",
            Description = "New Description"
        };

        User user = new()
        {
            Id = userId,
            Email = "test@example.com",
            Fullname = "Test User",
            Role = (byte)UserRolesEnum.User
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateProjectRequestDto>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _projectPoliciesMock
            .Setup(x => x.DuplicateTitle(userId, dto.Title))
            .Returns(false);

        _unitOfWorkMock
            .Setup(x => x.Projects.GetByIdAsync(dto.ProjectId))
            .ReturnsAsync((Project?)null);

        _unitOfWorkMock
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateProject(dto));
    }
}
