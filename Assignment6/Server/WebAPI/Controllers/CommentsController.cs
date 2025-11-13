using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(
        ICommentRepository commentRepo,
        IUserRepository userRepo,
        IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto request)
    {
        // (optional) validate existence
        if (userRepo.GetManyAsync().All(u => u.Id != request.UserId))
            return BadRequest($"User with id {request.UserId} does not exist.");
        if (postRepo.GetManyAsync().All(p => p.Id != request.PostId))
            return BadRequest($"Post with id {request.PostId} does not exist.");

        var created = await commentRepo.AddAsync(new Comment
        {
            UserId = request.UserId,
            PostId = request.PostId,
            Body = request.Body
        });

        var dto = new CommentDto
        {
            Id = created.Id,
            UserId = created.UserId,
            PostId = created.PostId,
            Body = created.Body
        };

        return Created($"/comments/{dto.Id}", dto);
    }

    // GET /comments
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany(
        [FromQuery] int? userId,
        [FromQuery] string? username,
        [FromQuery] int? postId)
    {
        var comments = commentRepo.GetManyAsync();

        if (userId.HasValue)
            comments = comments.Where(c => c.UserId == userId);

        if (postId.HasValue)
            comments = comments.Where(c => c.PostId == postId);

        if (!string.IsNullOrWhiteSpace(username))
        {
            var matchingUserIds = userRepo.GetManyAsync()
                .Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToHashSet();
            comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
        }

        var dtos = comments.Select(c => new CommentDto
        {
            Id = c.Id,
            UserId = c.UserId,
            PostId = c.PostId,
            Body = c.Body
        }).ToList();

        return Ok(dtos);
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingle(int id)
    {
        try
        {
            var c = await commentRepo.GetSingleAsync(id);
            return Ok(new CommentDto { Id = c.Id, UserId = c.UserId, PostId = c.PostId, Body = c.Body });
        }
        catch
        {
            return NotFound();
        }
    }

    // PUT /comments/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateCommentDto request)
    {
        try
        {
            var c = await commentRepo.GetSingleAsync(id);
            c.Body = request.Body;
            await commentRepo.UpdateAsync(c);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }
}
