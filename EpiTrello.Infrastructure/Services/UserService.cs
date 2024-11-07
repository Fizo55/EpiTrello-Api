using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Factories;

namespace EpiTrello.Infrastructure.Services;

public class UserService
{
    private readonly IGenericDao<User> _userDao;

    public UserService(DaoFactory daoFactory)
    {
        _userDao = daoFactory.CreateDao<User>();
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        return await _userDao.GetByPredicateAsync(s => s.Username == username) != null;
    }

    public async Task<User?> GetUserAsync(string username, string password)
    {
        return await _userDao.GetByPredicateAsync(s => s.Username == username && s.Password == password);
    }

    public async Task<IEnumerable<User>> GetAllUserAsync()
    {
        return await _userDao.GetAllAsync();
    }

    public async Task CreateUserAsync(User board)
    {
        await _userDao.AddAsync(board);
    }

    public async Task UpdateUserAsync(User board)
    {
        await _userDao.UpdateAsync(board);
    }

    public async Task DeleteUserAsync(User board)
    {
        await _userDao.DeleteAsync(board);
    }
}