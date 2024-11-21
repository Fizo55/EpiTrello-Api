using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Factories;

namespace EpiTrello.Infrastructure.Services;

public class BoardService
{
    private readonly IGenericDao<Board> _boardDao;

    public BoardService(DaoFactory daoFactory)
    {
        _boardDao = daoFactory.CreateDao<Board>();
    }
    
    public async Task<Board?> GetBoardAsync(long id, long userId)
    {
        return await _boardDao.GetByPredicateAsync(s => s.Id == id && s.UserIds.Contains(userId));
    }

    public async Task<Board?> GetBoardAsync(long id)
    {
        return await _boardDao.GetByIdAsync(id);
    }
    
    public async Task<Board?> GetBoardWithDetailsAsync(long id, long userId)
    {
        return await _boardDao.GetSingleOrDefaultAsync(
            b => b.Id == id && b.UserIds.Contains(userId),
            b => b.Stages,
            b => b.Blocks);
    }
    
    public async Task<IEnumerable<Board>> GetAllBoardsWithDetailsAsync(long userId)
    {
        return await _boardDao.GetAllWithIncludesAsync(
            b => b.UserIds.Contains(userId),
            b => b.Stages,
            b => b.Blocks);
    }

    public async Task CreateBoardAsync(Board board)
    {
        await _boardDao.AddAsync(board);
    }

    public async Task UpdateBoardAsync(Board board)
    {
        await _boardDao.UpdateAsync(board);
    }

    public async Task DeleteBoardAsync(Board board)
    {
        await _boardDao.DeleteAsync(board);
    }
}