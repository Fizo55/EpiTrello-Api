using System.Linq.Expressions;
using EpiTrello.Core.Interfaces;
using EpiTrello.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EpiTrello.Infrastructure.Dao;

public class GenericDao<T> : IGenericDao<T> where T : class
{
    private readonly EpiTrelloContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericDao(EpiTrelloContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
    
    public async Task<IEnumerable<T>> GetListByPredicateAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        params Func<IQueryable<T>, IQueryable<T>>[] includePaths)
    {
        IQueryable<T> query = includePaths.Aggregate<Func<IQueryable<T>, IQueryable<T>>?, IQueryable<T>>(_dbSet, (current, includePath) => includePath(current));
        return await query.SingleOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAllWithIncludesAsync(
        Expression<Func<T, bool>>? predicate,
        params Func<IQueryable<T>, IQueryable<T>>[] includePaths)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        query = includePaths.Aggregate(query, (current, includePath) => includePath(current));
        return await query.ToListAsync();
    }

}
