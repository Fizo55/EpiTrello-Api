using System.Linq.Expressions;

namespace EpiTrello.Core.Interfaces;

public interface IGenericDao<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
}
