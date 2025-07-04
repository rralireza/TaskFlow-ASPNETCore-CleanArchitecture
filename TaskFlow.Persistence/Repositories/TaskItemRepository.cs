using TaskFlow.Application.Intefaces.Repositories;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Persistence.Repositories;

public class TaskItemRepository : GenericRepository<TaskItem, Guid>, ITaskItemRepository
{
    public TaskItemRepository(TaskFlowDbContext context) : base(context)
    {
        
    }
}
