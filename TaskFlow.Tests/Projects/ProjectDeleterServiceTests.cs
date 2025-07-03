using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.Tracing;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Projects;

public class ProjectDeleterServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<ProjectDeleterService>> _loggerMock = new();

    private readonly ProjectDeleterService _service;

    public ProjectDeleterServiceTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILogger<ProjectDeleterService>>();

        _service = new ProjectDeleterService(_unitOfWork.Object, _currentUserServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task DeleteProject_ShouldDeleteProject_WhenUserIsOwnerOrAdmin()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();

        User user = new()
        {
            Id = userId,
            Email = "test@example.com",
            Fullname = "Test User",
            Role = (byte)UserRolesEnum.Admin,
        };

        Project project = new()
        {
            Id = projectId,
            Title = "Test Project",
            Description = "Test Description",
            CreatedByUserId = userId,
            CreatedAt = DateTime.Now,
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _unitOfWork
            .Setup(x => x.Projects.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        _unitOfWork
            .Setup(x => x.SaveAsync())
            .ReturnsAsync(1);

        //Act
        var result = await _service.DeleteProject(projectId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(project.Title, result.Title);

        _unitOfWork.Verify(x => x.Projects.Delete(project), Times.Once);
        _unitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        _loggerMock.Verify(x => x.Log(
        LogLevel.Information,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("deleted successfully")),
        null,
        It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        //Arrange
        Guid projectId = Guid.NewGuid();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        //Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteProject(projectId));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("unauthenticated user")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowUnauthorizedAccessException_WhenUserDoesNotExists()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        //Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteProject(projectId));

        _loggerMock.Verify(x => x.Log(
       LogLevel.Warning,
       It.IsAny<EventId>(),
       It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("non-existent")),
       null,
       It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowKeyNotFoundException_WhenProjectDoesNotExists()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();

        User user = new()
        {
            Id = userId,
            Email = "test@example.com",
            Fullname = "Test User",
            Role = (byte)UserRolesEnum.Admin,
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _unitOfWork
            .Setup(x => x.Projects.GetByIdAsync(projectId))
            .ReturnsAsync((Project?)null);

        //Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteProject(projectId));
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwnerOrAdmin()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();

        User user = new()
        {
            Id = userId,
            Email = "test@example.com",
            Fullname = "Test User",
            Role = (byte)UserRolesEnum.User
        };

        Project project = new()
        {
            Id = projectId,
            CreatedByUserId = otherUserId,
            Title = "Test Project",
            Description = "Test Description",
            CreatedAt = DateTime.Now
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWork
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _unitOfWork
            .Setup(x => x.Projects.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        //Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteProject(projectId));
    }
}
