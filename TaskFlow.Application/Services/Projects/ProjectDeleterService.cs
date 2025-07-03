using Microsoft.Extensions.Logging;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Projects;

public class ProjectDeleterService : IProjectDeleterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProjectDeleterService> _logger;

    public ProjectDeleterService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<ProjectDeleterService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }


    public async Task<ProjectResponseDto> DeleteProject(Guid projectId)
    {

        Guid? currentUserId = _currentUserService.UserId ?? null;

        if (currentUserId == null)
        {
            _logger.LogWarning($"Attempt to delete project {projectId} by an unauthenticated user.");
            throw new UnauthorizedAccessException("You must be logged in to delete a project.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(currentUserId.Value);

        if (user == null)
        {
            _logger.LogWarning($"Attempt to delete project {projectId} by a non-existent user {currentUserId}.");
            throw new UnauthorizedAccessException("You must be logged in to delete a project.");
        }

        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);

        if (project == null)
        {
            _logger.LogWarning($"Attempt to delete a non-existent project {projectId} by user {currentUserId}.");
            throw new KeyNotFoundException("Project not found.");
        }

        //Validation: check if the user is the creator of the project or an admin
        if (project.CreatedByUserId != user.Id || user.Role != (byte)UserRolesEnum.Admin)
        {
            _logger.LogWarning($"User {currentUserId} attempted to delete project {projectId} they do not own.");
            throw new UnauthorizedAccessException("You do not have permission to delete this project.");
        }

        _unitOfWork.Projects.Delete(project);

        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Project {project.Title} deleted successfully by user {user.Fullname}.");

        return new ProjectResponseDto
        {
            ProjectId = project.Id,
            Title = project.Title,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            CreateByUser = user.Fullname
        };

    }
}
