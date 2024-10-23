using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;

namespace EpiTrello.Core.Services;

public class BoardService
{
    private readonly IGenericDao<Board> _boardDao;

    public BoardService(IGenericDao<Board> boardDao)
    {
        _boardDao = boardDao;
    }

    public async Task<Board?> GetBoardAsync(int id)
    {
        return await _boardDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Board>> GetAllBoardsAsync()
    {
        return await _boardDao.GetAllAsync();
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