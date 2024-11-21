using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Factories;

namespace EpiTrello.Infrastructure.Services;

public class BlockService
{
    private readonly IGenericDao<Block> _blockDao;

    public BlockService(DaoFactory daoFactory)
    {
        _blockDao = daoFactory.CreateDao<Block>();
    }
        
    public async Task AddBlockAsync(Block block)
    {
        await _blockDao.AddAsync(block);
    }

    public async Task<Block?> GetBlockAsync(long boardId, int blockId)
    {
        return await _blockDao.GetSingleOrDefaultAsync(
            b => b.Id == blockId && b.BoardId == boardId);
    }

    public async Task UpdateBlockAsync(Block block)
    {
        await _blockDao.UpdateAsync(block);
    }
}