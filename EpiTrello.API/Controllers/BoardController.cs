using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BoardController : ControllerBase
{
    private readonly BoardService _boardService;

    public BoardController(BoardService boardService)
    {
        _boardService = boardService;
    }

    // GET: /board
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
    {
        var boards = await _boardService.GetAllBoardsAsync();
        return Ok(boards);
    }

    // GET: /board/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Board>> GetBoard(int id)
    {
        var board = await _boardService.GetBoardAsync(id);
        if (board == null)
        {
            return NotFound();
        }

        return Ok(board);
    }

    // POST: /board
    [HttpPost]
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
