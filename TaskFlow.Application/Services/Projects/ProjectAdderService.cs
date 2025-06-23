using FluentValidation;
using FluentValidation.Results;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.Helpers;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Projects;

public class ProjectAdderService : IProjectAdderService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IProjectPoliciesService _projectPolicies;

    private readonly IValidator<AddProjectRequestDto> _addProjectValidator;

    private readonly ICurrentUserService _currentUser;

    private const int MaxProjectsPerMonthForNormalUser = 4;
    public ProjectAdderService(IUnitOfWork unitOfWork, IValidator<AddProjectRequestDto> addProjectValidator, ICurrentUserService currentUser, IProjectPoliciesService projectPolicies)
    {
        _unitOfWork = unitOfWork;
        _addProjectValidator = addProjectValidator;
        _currentUser = currentUser;
        _projectPolicies = projectPolicies;
    }

    public async Task<ProjectResponseDto> CreateProject(AddProjectRequestDto request)
    {
        var validationResult = _addProjectValidator.Validate(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        Guid? currentUserId = _currentUser.UserId.HasValue ? _currentUser.UserId.Value : null;

        var currentUser = currentUserId.HasValue ? await _unitOfWork.Users.GetByIdAsync(currentUserId.Value)
            : null;

        if (_projectPolicies.DuplicateTitle(currentUserId.Value, request.Title))
        {
            throw new ValidationException(new List<ValidationFailure>
            {
                new("Title", "This title already exists!")
            });
        }

        if (!_projectPolicies.CanCreateProject(currentUserId.Value, (byte)UserRolesEnum.User, MaxProjectsPerMonthForNormalUser))
        {
            throw new ValidationException(new List<ValidationFailure>
            {
                new("ProjectLimit", "You have reached the project limit for this month!")
            });
        }

        Project project = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = currentUserId
        };

        await _unitOfWork.Projects.AddAsync(project);

        await _unitOfWork.SaveAsync();

        return new ProjectResponseDto
        {
            ProjectId = project.Id,
            Title = project.Title,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            CreateByUser = currentUser?.Email ?? "Unknown",
            CreateByUserRole = ((UserRolesEnum)currentUser.Role).GetDescription()
        };
    }
}
