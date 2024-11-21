using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Factories;

namespace EpiTrello.Infrastructure.Services;

public class StageService
{
    private readonly IGenericDao<Stage> _stageDao;
    private readonly IGenericDao<Board> _boardDao;

    public StageService(DaoFactory daoFactory)
    {
        _stageDao = daoFactory.CreateDao<Stage>();
        _boardDao = daoFactory.CreateDao<Board>();
    }

    public async Task AddStageAsync(Stage stage)
    {
        await _stageDao.AddAsync(stage);
    }
    
    public async Task<Stage?> GetStageAsync(long boardId, int stageId, long userId)
    {
        var board = await _boardDao.GetByPredicateAsync(s => s.Id == boardId && s.UserIds.Contains(userId));

        if (board == null)
        {
            return null;
        }
        
        return await _stageDao.GetSingleOrDefaultAsync(
            s => s.Id == stageId && s.BoardId == boardId);
    }
}