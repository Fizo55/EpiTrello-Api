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

    public async Task<Board?> GetBoardAsync(int id)
    {
        return await _boardDao.GetByIdAsync(id);
    }
    
    public async Task<Board?> GetBoardWithDetailsAsync(int id)
    {
        return await _boardDao.GetSingleOrDefaultAsync(
            b => b.Id == id,
            b => b.Stages,
            b => b.Blocks);
    }

    public async Task<IEnumerable<Board>> GetAllBoardsAsync()
    {
        return await _boardDao.GetAllAsync();
    }
    
    public async Task<IEnumerable<Board>> GetAllBoardsWithDetailsAsync()
    {
        return await _boardDao.GetAllWithIncludesAsync(
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