using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services.Projects;

public class ProjectPoliciesService : IProjectPoliciesService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectPoliciesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public bool CanCreateProject(Guid userId, byte role, int maxProjects)
    {
        if (role == (byte)UserRolesEnum.Admin)
            return true;

        DateTime now = DateTime.UtcNow;
        DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

        var projectsCount = _unitOfWork
            .Projects
            .WhereQueryable(p => p.CreatedByUserId == userId 
            && p.CreatedAt >= startOfMonth 
            && p.CreatedAt <= endOfMonth)
            .Count();

        return projectsCount < maxProjects;
    }

    public bool DuplicateTitle(Guid userId, string title)
    {
        return _unitOfWork
            .Projects
            .WhereQueryable(x => x.CreatedByUserId == userId)
            .Any(x => x.Title == title);
    }
}
