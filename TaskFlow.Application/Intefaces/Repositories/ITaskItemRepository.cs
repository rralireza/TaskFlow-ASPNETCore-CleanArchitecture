using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Intefaces.Repositories;

public interface ITaskItemRepository : IGenericRepository<TaskItem, Guid>
{
}
