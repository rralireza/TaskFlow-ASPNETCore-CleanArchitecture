using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TaskFlow.Application.Intefaces.Repositories;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Persistence.Repositories;

public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
{
    private readonly TaskFlowDbContext _context;

    private readonly DbSet<T> _dbSet;

    public GenericRepository(TaskFlowDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(TKey id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<T> GetAllIncuding(params Expression<Func<T, object>>[] includingProperties)
    {
        IQueryable<T> query = _dbSet;

        foreach (var property in includingProperties)
        {
            query = query.Include(property);
        }

        return query;
    }

    public IQueryable<T> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public T? GetById(TKey id)
    {
        return _dbSet.Find(id);
    }

    public async Task<T?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public IQueryable<T> WhereIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includingProperties)
    {
        IQueryable<T> query = _dbSet;

        foreach (var property in includingProperties)
            query = query.Include(property);

        return query.Where(predicate);
    }

    public IQueryable<T> WhereQueryable(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }
}
