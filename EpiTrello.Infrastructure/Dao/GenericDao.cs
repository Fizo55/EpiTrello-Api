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

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
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
}
