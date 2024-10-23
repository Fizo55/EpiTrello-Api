using EpiTrello.Core.Interfaces;
using EpiTrello.Infrastructure.Dao;
using EpiTrello.Infrastructure.Data;

namespace EpiTrello.Infrastructure.Factories;

public class DaoFactory
{
    private readonly EpiTrelloContext _context;

    public DaoFactory(EpiTrelloContext context)
    {
        _context = context;
    }

    public IGenericDao<T> CreateDao<T>() where T : class
    {
        return new GenericDao<T>(_context);
    }
}
