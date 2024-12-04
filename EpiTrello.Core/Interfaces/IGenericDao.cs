using System.Linq.Expressions;

namespace EpiTrello.Core.Interfaces;

public interface IGenericDao<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetListByPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IQueryable<T>>[] includePaths);
    Task<IEnumerable<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>>? predicate, params Func<IQueryable<T>, IQueryable<T>>[] includePaths);
}
