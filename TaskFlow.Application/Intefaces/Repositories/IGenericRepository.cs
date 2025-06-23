using System.Linq.Expressions;

namespace TaskFlow.Application.Intefaces.Repositories;

public interface IGenericRepository<T, TKey> where T : class
{
    T? GetById(TKey id);
    Task<T?> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> GetAllAsync();
    IEnumerable<T> GetAll();
    IQueryable<T> GetAllQueryable();
    IQueryable<T> GetAllIncuding(params Expression<Func<T, object>>[] includingProperties);
    IQueryable<T> WhereQueryable(Expression<Func<T, bool>> predicate);
    IQueryable<T> WhereIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includingProperties);
    Task AddAsync(T entity);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(TKey id);
    void Save();
    Task SaveAsync();
}
