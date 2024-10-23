using System.Linq.Expressions;

namespace EpiTrello.Core.Interfaces;

public interface IGenericDao<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
