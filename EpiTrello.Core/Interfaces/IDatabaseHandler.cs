using System.Linq.Expressions;

namespace EpiTrello.Core.Interfaces;

public interface IDatabaseHandler
{
    Task<T> AddAsync<T>(T entity) where T : class;
    Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> expression) where T : class;
    Task<List<T>> GetAllAsync<T>() where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task DeleteAsync<T>(T entity) where T : class;
    Task<IEnumerable<T>> GetAllWithIncludesAsync<T>(
        Expression<Func<T, bool>> predicate,
        params Func<IQueryable<T>, IQueryable<T>>[] includes
    ) where T : class;
}