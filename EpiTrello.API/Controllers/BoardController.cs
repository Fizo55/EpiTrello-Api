using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BoardController : BaseController
{
    private readonly BoardService _boardService;
    private readonly BlockService _blockService;

    public BoardController(BoardService boardService, BlockService blockService)
    {
        _boardService = boardService;
        _blockService = blockService;
    }

    // GET: /board
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
    {
        var boards = await _boardService.GetAllBoardsWithDetailsAsync();
        return Ok(boards);
    }
    
    [HttpPost("{boardId}/blocks")]
    public async Task<ActionResult<Block>> CreateBlock(int boardId, [FromBody] Block block)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var board = await _boardService.GetBoardAsync(boardId);
        if (board == null)
        {
            return NotFound("Board not found");
        }

        block.BoardId = boardId;
        await _blockService.AddBlockAsync(block);

        return CreatedAtAction(nameof(GetBlock), new { boardId, blockId = block.Id }, block);
    }
    
    [HttpGet("{boardId}/blocks/{blockId}")]
    public async Task<ActionResult<Block>> GetBlock(int boardId, int blockId)
    {
        var block = await _blockService.GetBlockAsync(boardId, blockId);
        if (block == null)
        {
            return NotFound();
        }

        return Ok(block);
    }
    
    [HttpPut("{boardId}/blocks/{blockId}")]
    public async Task<IActionResult> UpdateBlockStatus(int boardId, int blockId, [FromBody] Block block)
    {
        if (blockId != block.Id)
        {
            return BadRequest("Block ID mismatch");
        }

        var existingBlock = await _blockService.GetBlockAsync(boardId, blockId);
        if (existingBlock == null)
        {
            return NotFound();
        }

        existingBlock.Status = block.Status;
        await _blockService.UpdateBlockAsync(existingBlock);
        return NoContent();
    }

    // GET: /board/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Board>> GetBoard(int id)
    {
        var board = await _boardService.GetBoardWithDetailsAsync(id);
        if (board == null)
        {
            return NotFound();
        }

        return Ok(board);
    }

    // POST: /board/createboard
    [HttpPost("createboard")]
    public async Task<ActionResult<Board>> CreateBoard([FromBody] Board board)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _boardService.CreateBoardAsync(board);
        return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
    }

    // PUT: /board/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(int id, [FromBody] Board board)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != board.Id)
        {
            return BadRequest("Board ID mismatch");
        }

        var existingBoard = await _boardService.GetBoardAsync(id);
        if (existingBoard == null)
        {
            return NotFound();
        }

        await _boardService.UpdateBoardAsync(board);
        return NoContent();
    }

    // DELETE: /board/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(int id)
    {
        var board = await _boardService.GetBoardAsync(id);
        if (board == null)
        {
            return NotFound();
        }

        await _boardService.DeleteBoardAsync(board);
        return NoContent();
    }
}
