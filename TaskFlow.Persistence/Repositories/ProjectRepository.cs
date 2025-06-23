using TaskFlow.Application.Intefaces.Repositories;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Persistence.Repositories;

public class ProjectRepository : GenericRepository<Project, Guid>, IProjectRepository
{
    public ProjectRepository(TaskFlowDbContext context) : base(context)
    {
        
    }
}
