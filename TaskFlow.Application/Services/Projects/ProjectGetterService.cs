using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.DTO.Pagination;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.Helpers;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Projects;

public class ProjectGetterService : IProjectGetterService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICurrentUserService _currentUserService;

    public ProjectGetterService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PagingResult<ProjectResponseDto>> GetAllProjectsForCurrentUser(ProjectFilterDto filter)
    {
        if (!_currentUserService.UserId.HasValue)
            throw new ArgumentNullException("User is null!");

        Guid? currentUserId = _currentUserService.UserId.Value;

        User? currentUser = currentUserId != null ? await _unitOfWork.Users.GetByIdAsync(currentUserId.Value) : null;

        if (currentUser is null)
            throw new ArgumentNullException("Cannot find user!");

        IQueryable<Project> projects = _unitOfWork
           .Projects
           .GetAllIncuding(u => u.CreatedByUser)
           .Where(x => x.CreatedByUserId == currentUserId);

        if (!string.IsNullOrWhiteSpace(filter.Title))
            projects = projects.Where(p => p.Title.Contains(filter.Title));

        if (filter.FromDate.HasValue)
            projects = projects.Where(p => p.CreatedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            projects = projects.Where(p => p.CreatedAt <= filter.ToDate.Value);

        if (filter.PageNumber <= 0) filter.PageNumber = 1;
        if (filter.PageSize <= 0) filter.PageSize = 10;

        int totalCount = projects.Count();

        int maxPage = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        if (filter.PageNumber > maxPage)
            filter.PageNumber = maxPage;

        var data = projects
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProjectResponseDto
            {
                ProjectId = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                CreateByUser = p.CreatedByUser.Fullname,
                CreateByUserRole = ((UserRolesEnum)p.CreatedByUser.Role).GetDescription()
            }).ToList();

        return new PagingResult<ProjectResponseDto>
        {
            Items = data,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
}
