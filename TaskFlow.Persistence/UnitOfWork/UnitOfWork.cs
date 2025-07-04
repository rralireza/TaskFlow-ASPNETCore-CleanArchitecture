using TaskFlow.Application.Intefaces.Repositories;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly TaskFlowDbContext _context;

    public IUserRepository Users { get; }

    public IProjectRepository Projects { get; }

    public ITaskItemRepository Tasks { get; }

    IUserRepository IUnitOfWork.Users => Users;

    public UnitOfWork(TaskFlowDbContext context, IUserRepository users, IProjectRepository projects, ITaskItemRepository tasks)
    {
        _context = context;
        Users = users;
        Projects = projects;
        Tasks = tasks;
    }

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
}
