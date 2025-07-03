using FluentValidation;
using FluentValidation.Results;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Projects;

public class ProjectUpdaterService : IProjectUpdaterService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<UpdateProjectRequestDto> _validator;
    private readonly IProjectPoliciesService _projectPolicies;

    public ProjectUpdaterService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IValidator<UpdateProjectRequestDto> validator, IProjectPoliciesService projectPolicies)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _validator = validator;
        _projectPolicies = projectPolicies;
    }

    public async Task<ProjectResponseDto> UpdateProject(UpdateProjectRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        Guid? currentUserId = _currentUserService.UserId;

        var currentUser = currentUserId.HasValue ? await _unitOfWork.Users.GetByIdAsync(currentUserId.Value)
            : null;

        if (_projectPolicies.DuplicateTitle(currentUserId.Value, request.Title))
        {
            throw new ValidationException(new List<ValidationFailure>
            {
                new("Title", "This title already exists!")
            });
        }

        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);

        if (project == null)
            throw new ArgumentNullException("Project not exsists!");

        project.Title = request.Title;
        project.Description = request.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveAsync();

        return new ProjectResponseDto
        {
            ProjectId = project.Id,
            Title = project.Title,
            Description = project.Description,
            CreateByUser = currentUser?.Email,
            CreateByUserRole = currentUser?.Role != null ?((UserRolesEnum)currentUser?.Role).ToString() : "Unknown",
            CreatedAt = project.CreatedAt
        };
    }
}
