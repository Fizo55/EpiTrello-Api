using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Factories;

namespace EpiTrello.Infrastructure.Services;

public class BlockService
{
    private readonly IGenericDao<Block> _blockDao;
    private readonly IGenericDao<Board> _boardDao;
    private readonly IGenericDao<Ticket> _ticketDao;

    public BlockService(DaoFactory daoFactory)
    {
        _blockDao = daoFactory.CreateDao<Block>();
        _boardDao = daoFactory.CreateDao<Board>();
        _ticketDao = daoFactory.CreateDao<Ticket>();
    }
        
    public async Task AddBlockAsync(Block block)
    {
        await _blockDao.AddAsync(block);
    }

    public async Task<Block?> GetBlockAsync(long boardId, int blockId, long userId)
    {
        var board = await _boardDao.GetByPredicateAsync(s => s.Id == boardId && s.UserIds.Contains(userId));

        if (board == null)
        {
            return null;
        }
        
        return await _blockDao.GetSingleOrDefaultAsync(
            b => b.Id == blockId && b.BoardId == boardId);
    }

    public async Task UpdateBlockAsync(Block block)
    {
        await _blockDao.UpdateAsync(block);
    }
    
    public async Task AddTicketAsync(long blockId, Ticket ticket)
    {
        ticket.BlockId = blockId;
        await _ticketDao.AddAsync(ticket);
    }

    public async Task<List<Ticket>> GetTicketsAsync(long blockId)
    {
        return (await _ticketDao.GetListByPredicateAsync(t => t.BlockId == blockId)).ToList();
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        await _ticketDao.UpdateAsync(ticket);
    }

    public async Task DeleteTicketAsync(long ticketId)
    {
        var ticket = await _ticketDao.GetSingleOrDefaultAsync(t => t.Id == ticketId);
        if (ticket != null)
        {
            await _ticketDao.DeleteAsync(ticket);
        }
    }
}