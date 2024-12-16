using System.Linq.Expressions;
using EpiTrello.Core.Interfaces;
using EpiTrello.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EpiTrello.Infrastructure;

public class DatabaseHandler : IDatabaseHandler
{
    private readonly IDbContextFactory<EpiTrelloContext> _contextFactory;

    public DatabaseHandler(IDbContextFactory<EpiTrelloContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<T> AddAsync<T>(T entity) where T : class
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> expression) where T : class
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().Where(expression).ToListAsync();
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().ToListAsync();
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(T entity) where T : class
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<T>> GetAllWithIncludesAsync<T>(
        Expression<Func<T, bool>>? predicate,
        params Func<IQueryable<T>, IQueryable<T>>[] includes
    ) where T : class
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        IQueryable<T> query = context.Set<T>();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        query = includes.Aggregate(query, (current, include) => include(current));
        return await query.ToListAsync();
    }
}