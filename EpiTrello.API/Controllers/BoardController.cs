using System.Security.Claims;
using EpiTrello.API.Requests;
using EpiTrello.Core.Interfaces;
using EpiTrello.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BoardController : BaseController
{
    public IConfiguration _configureation;
    public IDatabaseHandler _dbHandler;

    public BoardController(IConfiguration configuration, IDatabaseHandler dbHandler)
    {
        _configureation = configuration;
        _dbHandler = dbHandler;
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();

        if (user == null)
        {
            return Unauthorized();
        }
        
        var boards = await _dbHandler.GetAllWithIncludesAsync<Board>(b => b.UserIds.Contains(user.Id),
            query => query.Include(b => b.Blocks).ThenInclude(block => block.Tickets),
            query => query.Include(b => b.Stages));
        return Ok(boards);
    }
    
    // PUT: /board/{boardId}/stages/order
    [HttpPut("{boardId}/stages/order")]
    public async Task<IActionResult> UpdateStagesOrder(long boardId, [FromBody] UpdateStagesOrderRequest request, [FromServices] WebSocketManager webSocketManager)
    {
        if (request.Stages.Count == 0)
        {
            return BadRequest("No stages provided for reorder.");
        }
    
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == boardId && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound("Board not found");
        }

        var stages = await _dbHandler.GetAsync<Stage>(s => s.BoardId == boardId);
        var stageDictionary = stages.ToDictionary(s => s.Id, s => s);

        foreach (var updatedStage in request.Stages)
        {
            if (stageDictionary.TryGetValue(updatedStage.Id, out var existingStage))
            {
                existingStage.Place = updatedStage.Place;
                await _dbHandler.UpdateAsync(existingStage);
            }
            else
            {
                return BadRequest($"Stage with ID {updatedStage.Id} does not exist in this board.");
            }
        }

        var reorderedStages = await _dbHandler.GetAsync<Stage>(s => s.BoardId == boardId);
        
        var update = new { message = $"{username}:stage_reordered", reorderedStages };
        await webSocketManager.NotifyAsync(boardId, update);

        return NoContent();
    }
    
    // POST: /board/{boardId}/stages
    [HttpPost("{boardId}/stages")]
    public async Task<IActionResult> CreateStage(long boardId, [FromBody] Stage stage, [FromServices] WebSocketManager webSocketManager)
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == boardId && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound("Board not found");
        }

        var latestStages = (await _dbHandler.GetAsync<Stage>(s => s.BoardId == boardId)).OrderByDescending(s => s.Place).FirstOrDefault();

        stage.Place = latestStages?.Place + 1 ?? 0;
        stage.BoardId = boardId;
        Stage createdStage = await _dbHandler.AddAsync(stage);
        
        var update = new { message = $"{username}:stage_created", createdStage };
        await webSocketManager.NotifyAsync(boardId, update);

        return Ok(createdStage);
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }
        
        var userHaveBoard = (await _dbHandler.GetAsync<Board>(s => s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (userHaveBoard == null)
        {
            return NotFound();
        }

        var stage = (await _dbHandler.GetAsync<Stage>(s => s.Id == stageId && s.BoardId == boardId)).FirstOrDefault();
        if (stage == null)
        {
            return NotFound("Stage not found");
        }

        return Ok(stage);
    }
    
    [HttpPost("{boardId}/blocks/{blockId}/tickets")]
    public async Task<ActionResult<Ticket>> CreateTicket(long boardId, int blockId, [FromBody] Ticket ticket)
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

        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == boardId && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound("Board not found");
        }

        var userHaveBoard = (await _dbHandler.GetAsync<Board>(s => s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (userHaveBoard == null)
        {
            return NotFound();
        }

        var existingBlock = (await _dbHandler.GetAsync<Block>(s => s.Id == blockId && s.BoardId == boardId)).FirstOrDefault();
        if (existingBlock == null)
        {
            return NotFound("Block not found");
        }

        ticket.BlockId = blockId;
        await _dbHandler.AddAsync(ticket);

        return CreatedAtAction(nameof(GetTickets), new { boardId, blockId }, ticket);
    }
    
    [HttpGet("{boardId}/blocks/{blockId}/tickets")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets(long boardId, int blockId)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == boardId && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound("Board not found");
        }
        
        var userHaveBoard = (await _dbHandler.GetAsync<Board>(s => s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (userHaveBoard == null)
        {
            return NotFound();
        }

        var existingBlock = (await _dbHandler.GetAsync<Block>(s => s.Id == blockId && s.BoardId == boardId)).FirstOrDefault();
        if (existingBlock == null)
        {
            return NotFound("Block not found");
        }

        var tickets = await _dbHandler.GetAsync<Ticket>(s => s.BlockId == blockId);
        return Ok(tickets);
    }
    
    // GET: /board/boardId/blocks
    [HttpPost("{boardId}/blocks")]
    public async Task<IActionResult> CreateBlock(long boardId, [FromBody] Block block, [FromServices] WebSocketManager webSocketManager)
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == boardId && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound("Board not found");
        }

        block.BoardId = boardId;
        Block createdBlock = await _dbHandler.AddAsync(block);

        var update = new { message = $"{username}:block_created", createdBlock };
        await webSocketManager.NotifyAsync(boardId, update);

        return Ok(createdBlock);
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        var userHaveBoard = (await _dbHandler.GetAsync<Board>(s => s.UserIds.Contains(user.Id))).FirstOrDefault();

        if (userHaveBoard == null)
        {
            return NotFound();
        }

        var existingBlock = (await _dbHandler.GetAsync<Block>(s => s.Id == blockId && s.BoardId == boardId)).FirstOrDefault();
        if (existingBlock == null)
        {
            return NotFound();
        }

        return Ok(existingBlock);
    }
    
    // PUT: /board/boardId/blocks/blockId
    [HttpPut("{boardId}/blocks/{blockId}")]
    public async Task<IActionResult> UpdateBlockStatus(long boardId, int blockId, [FromBody] Block block, [FromServices] WebSocketManager webSocketManager)
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }
        
        var userHaveBoard = (await _dbHandler.GetAsync<Board>(s => s.UserIds.Contains(user.Id))).FirstOrDefault();

        if (userHaveBoard == null)
        {
            return NotFound();
        }

        var existingBlock = (await _dbHandler.GetAsync<Block>(s => s.Id == blockId && s.BoardId == boardId)).FirstOrDefault();
        if (existingBlock == null)
        {
            return NotFound();
        }

        existingBlock.Status = block.Status;
        existingBlock.Title = block.Title;
        existingBlock.Description = block.Description;
        await _dbHandler.UpdateAsync(existingBlock);
        
        var update = new { message = $"{username}:block_reordered", existingBlock };
        await webSocketManager.NotifyAsync(boardId, update);
        
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }
        
        var board = (await _dbHandler.GetAllWithIncludesAsync<Board>(b => b.Id == id && b.UserIds.Contains(user.Id),
            query => query.Include(b => b.Blocks).ThenInclude(block => block.Tickets),
            query => query.Include(b => b.Stages))).FirstOrDefault();
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        board.UserIds = new[] { user.Id };

        await _dbHandler.AddAsync(board);
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }

        var existingBoard = (await _dbHandler.GetAsync<Board>(s => s.Id == id && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (existingBoard == null)
        {
            return NotFound();
        }

        await _dbHandler.UpdateAsync(board);
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
        
        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized();
        }
        
        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == id && s.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound();
        }

        await _dbHandler.DeleteAsync(board);
        return NoContent();
    }
    
    [HttpPost("{boardId}/invite/link")]
    public async Task<IActionResult> CreateInviteLink(long boardId, [FromBody] CreateInviteLinkRequest request)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
    
        User? currentUser = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(s => s.Id == boardId && s.UserIds.Contains(currentUser.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound();
        }

        var token = GenerateToken();
        var inviteLink = new InviteLink
        {
            BoardId = boardId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(request.DaysValid),
            Status = "Active"
        };

        await _dbHandler.AddAsync(inviteLink);

        var inviteUrl = $"{_configureation["WebsiteUrl"]}/board/invite/{token}/accept";
        
        return Ok(new { inviteUrl });
    }
    
    [HttpPost("invite/{token}/accept")]
    public async Task<IActionResult> AcceptInviteLink(string token)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        User? currentUser = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var inviteLink = (await _dbHandler.GetAsync<InviteLink>(i => i.Token == token && i.Status == "Active")).FirstOrDefault();
        if (inviteLink == null)
        {
            return NotFound();
        }

        if (DateTime.UtcNow > inviteLink.ExpiresAt)
        {
            inviteLink.Status = "Expired";
            await _dbHandler.UpdateAsync(inviteLink);
            return BadRequest();
        }

        var board = (await _dbHandler.GetAsync<Board>(b => b.Id == inviteLink.BoardId)).FirstOrDefault();
        if (board == null)
        {
            return NotFound();
        }

        var userIds = board.UserIds.ToList();
        if (!userIds.Contains(currentUser.Id))
        {
            userIds.Add(currentUser.Id);
            board.UserIds = userIds.ToArray();
            await _dbHandler.UpdateAsync(board);
        }

        return Ok();
    }
    
    // GET: /board/{boardId}/blocks/{blockId}/comments
    [HttpGet("{boardId}/blocks/{blockId}/comments")]
    public async Task<ActionResult<IEnumerable<object>>> GetComments(long boardId, int blockId)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(b => b.Id == boardId && b.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound();
        }

        var block = (await _dbHandler.GetAsync<Block>(b => b.Id == blockId && b.BoardId == boardId)).FirstOrDefault();
        if (block == null)
        {
            return NotFound();
        }

        var comments = await _dbHandler.GetAllWithIncludesAsync<Comment>(
            c => c.BlockId == blockId,
            query => query.Include(c => c.User)
        );

        var commentDtos = comments.Select(c => new {
            c.Id,
            c.Content,
            c.CreatedAt,
            c.User?.Username
        });

        return Ok(commentDtos);
    }

    // POST: /board/{boardId}/blocks/{blockId}/comments
    [HttpPost("{boardId}/blocks/{blockId}/comments")]
    public async Task<IActionResult> CreateComment(long boardId, int blockId, [FromBody] CreateCommentRequest request)
    {
        string? username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        User? user = (await _dbHandler.GetAsync<User>(s => s.Username == username)).FirstOrDefault();
        if (user == null)
        {
            return Unauthorized();
        }

        var board = (await _dbHandler.GetAsync<Board>(b => b.Id == boardId && b.UserIds.Contains(user.Id))).FirstOrDefault();
        if (board == null)
        {
            return NotFound();
        }

        var block = (await _dbHandler.GetAsync<Block>(b => b.Id == blockId && b.BoardId == boardId)).FirstOrDefault();
        if (block == null)
        {
            return NotFound();
        }

        var comment = new Comment
        {
            BlockId = blockId,
            UserId = user.Id,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _dbHandler.AddAsync(comment);
        return Ok();
    }

    private string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}
