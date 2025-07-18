using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.DTO.Pagination;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Application.Intefaces.Services.TaskItem;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services.TaskItem;

public sealed class TaskItemGetterService : ITaskItemGetterService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICurrentUserService _currentUserService;
    public TaskItemGetterService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PagingResult<TaskItemResponseDto>> GetAllTasksForCurrentUser(TaskFilterDto filter)
    {
        if (!_currentUserService.UserId.HasValue)
            throw new UnauthorizedAccessException("User is not authenticated!");

        Guid userId = _currentUserService.UserId.Value;

        User currentUser = await _unitOfWork
            .Users
            .GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found!");

        var tasks = _unitOfWork
            .Tasks
            .GetAllIncuding(p => p.Project, u => u.InsertUserDetails)
            .Where(x => x.InsertUser == userId || x.AssignedToUserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.Title))
            tasks = tasks.Where(x => x.Title.Contains(filter.Title));

        if (filter.FromDate.HasValue)
            tasks = tasks.Where(x => x.CreatedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            tasks = tasks.Where(x => x.CreatedAt <= filter.ToDate.Value);

        int totalCount = tasks.Count();

        if (filter.PageNumber <= 0) filter.PageNumber = 1;

        if (filter.PageSize <= 0) filter.PageSize = 10;

        int maxPage = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        if (filter.PageNumber > maxPage) filter.PageNumber = maxPage;

        var data = tasks
            .OrderByDescending(x => x.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new TaskItemResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DeadLine = x.Deadline,
                Status = x.Status.ToString(),
                Priority = x.Status.ToString(),
                Project = x.Project.Title
            }).ToList();

        return new PagingResult<TaskItemResponseDto>
        {
            Items = data,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
}
