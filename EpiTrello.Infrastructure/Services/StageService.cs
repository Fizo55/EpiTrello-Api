using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Factories;

namespace EpiTrello.Infrastructure.Services;

public class StageService
{
    private readonly IGenericDao<Stage> _stageDao;

    public StageService(DaoFactory daoFactory)
    {
        _stageDao = daoFactory.CreateDao<Stage>();
    }

    public async Task AddStageAsync(Stage stage)
    {
        await _stageDao.AddAsync(stage);
    }
    
    public async Task<Stage?> GetStageAsync(int boardId, int stageId)
    {
        return await _stageDao.GetSingleOrDefaultAsync(
            s => s.Id == stageId && s.BoardId == boardId);
    }
}