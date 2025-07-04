using TaskFlow.Application.Intefaces.Repositories;

namespace TaskFlow.Application.Intefaces.UnitOfWork;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    IProjectRepository Projects { get; }

    ITaskItemRepository Tasks { get; }

    Task<int> SaveAsync();
}
