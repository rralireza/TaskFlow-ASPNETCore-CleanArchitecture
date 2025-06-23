using Moq;
using System.Linq.Expressions;
using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.Helpers;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Projects;

public class ProjectGetterServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

    private readonly ProjectGetterService _service;

    public ProjectGetterServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _service = new ProjectGetterService(
            _unitOfWorkMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetAllProjectsForCurrentUser_ShouldReturnFilteredPagedProjects_WhenUserExists()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        User currentUser = new()
        {
            Id = userId,
            Fullname = "Test User Fullname",
            Email = "test@example.com",
            Role = (byte)UserRolesEnum.User
        };

        List<Project> projects = new()
        {
            new Project
            {
                Id = Guid.NewGuid(),
                Title = "Test Project 1",
                Description = "Test Project Description 1",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                CreatedByUser = currentUser
            },
            new Project
            {
                Id = Guid.NewGuid(),
                Title = "Test Project 2",
                Description = "Test Project Description 2",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                CreatedByUserId = userId,
                CreatedByUser = currentUser
            }
        };

        ProjectFilterDto filter = new()
        {
            PageNumber = 1,
            PageSize = 10,
            Title = "Test"
        };

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWorkMock
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync(currentUser);

        _unitOfWorkMock
         .Setup(x => x.Projects.GetAllIncuding(It.IsAny<Expression<Func<Project, object>>[]>()))
         .Returns(projects.AsQueryable());

        //Act
        var result = await _service.GetAllProjectsForCurrentUser(filter);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item =>
        {
            Assert.Contains("Test", item.Title);
            Assert.Equal(currentUser.Fullname, item.CreateByUser);
            Assert.Equal(((UserRolesEnum)currentUser.Role).GetDescription(), item.CreateByUserRole);
        });

    }


    [Fact]
    public async Task GetAllProjectsForCurrentUser_ShouldThrowArgumentNullException_WhenUserIdIsNull()
    {
        //Arrange
        ProjectFilterDto filter = new();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetAllProjectsForCurrentUser(filter));
    }

    [Fact]
    public async Task GetAllProjectsForCurrentUser_ShouldThrowArguemntNullException_WhenUserNotFound()
    {
        //Arrange
        Guid userId = Guid.NewGuid();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _unitOfWorkMock
            .Setup(x => x.Users.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetAllProjectsForCurrentUser(new ProjectFilterDto()));
    }
}
