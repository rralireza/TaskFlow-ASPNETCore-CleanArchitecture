using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Intefaces.Repositories;

public interface IUserRepository : IGenericRepository<User, Guid>
{

}
