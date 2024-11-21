using System.Security.Claims;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BoardController : BaseController
{
    private readonly UserService _userService;
    private readonly BoardService _boardService;
    private readonly BlockService _blockService;
    private readonly StageService _stageService;

    public BoardController(UserService userService, BoardService boardService, BlockService blockService, StageService stageService)
    {
        _userService = userService;
        _boardService = boardService;
        _blockService = blockService;
        _stageService = stageService;
    }

    // GET: /board
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }
        
        var boards = await _boardService.GetAllBoardsWithDetailsAsync(user.Id);
        return Ok(boards);
    }
    
    // POST: /board/{boardId}/stages
    [HttpPost("{boardId}/stages")]
    public async Task<ActionResult<Stage>> CreateStage(long boardId, [FromBody] Stage stage)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }

        var board = await _boardService.GetBoardAsync(boardId, user.Id);
        if (board == null)
        {
            return NotFound("Board not found");
        }

        stage.BoardId = boardId;
        await _stageService.AddStageAsync(stage);

        return CreatedAtAction(nameof(GetStage), new { boardId, stageId = stage.Id }, stage);
    }

    // GET: /board/{boardId}/stages/{stageId}
    [HttpGet("{boardId}/stages/{stageId}")]
    public async Task<ActionResult<Stage>> GetStage(long boardId, int stageId)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }
        
        var stage = await _stageService.GetStageAsync(boardId, stageId, user.Id);
        if (stage == null)
        {
            return NotFound();
        }

        return Ok(stage);
    }
    
    // GET: /board/boardId/blocks
    [HttpPost("{boardId}/blocks")]
    public async Task<ActionResult<Block>> CreateBlock(long boardId, [FromBody] Block block)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }

        var board = await _boardService.GetBoardAsync(boardId, user.Id);
        if (board == null)
        {
            return NotFound("Board not found");
        }

        block.BoardId = boardId;
        await _blockService.AddBlockAsync(block);

        return CreatedAtAction(nameof(GetBlock), new { boardId, blockId = block.Id }, block);
    }
    
    // GET: /board/boardId/blocks/blockId
    [HttpGet("{boardId}/blocks/{blockId}")]
    public async Task<ActionResult<Block>> GetBlock(long boardId, int blockId)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }

        var block = await _blockService.GetBlockAsync(boardId, blockId, user.Id);
        if (block == null)
        {
            return NotFound();
        }

        return Ok(block);
    }
    
    // PUT: /board/boardId/blocks/blockId
    [HttpPut("{boardId}/blocks/{blockId}")]
    public async Task<IActionResult> UpdateBlockStatus(long boardId, int blockId, [FromBody] Block block)
    {
        if (blockId != block.Id)
        {
            return BadRequest("Block ID mismatch");
        }
        
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }

        var existingBlock = await _blockService.GetBlockAsync(boardId, blockId, user.Id);
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
    public async Task<ActionResult<Board>> GetBoard(long id)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }
        
        var board = await _boardService.GetBoardWithDetailsAsync(id, user.Id);
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
        
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }

        board.UserIds = new[] { user.Id };

        await _boardService.CreateBoardAsync(board);
        return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
    }

    // PUT: /board/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(long id, [FromBody] Board board)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != board.Id)
        {
            return BadRequest("Board ID mismatch");
        }
        
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }

        var existingBoard = await _boardService.GetBoardAsync(id, user.Id);
        if (existingBoard == null)
        {
            return NotFound();
        }

        await _boardService.UpdateBoardAsync(board);
        return NoContent();
    }

    // DELETE: /board/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(long id)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        User? user = await _userService.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return Unauthorized();
        }
        
        var board = await _boardService.GetBoardAsync(id, user.Id);
        if (board == null)
        {
            return NotFound();
        }

        await _boardService.DeleteBoardAsync(board);
        return NoContent();
    }
}
