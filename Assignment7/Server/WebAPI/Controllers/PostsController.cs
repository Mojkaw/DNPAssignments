using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostDto request)
    {
        var author = userRepo.GetManyAsync().FirstOrDefault(u => u.Id == request.UserId);
        if (author is null)
            return BadRequest($"User with id {request.UserId} does not exist.");

        // Create entity using constructor
        var newPost = new Post(request.Title, request.Body, request.UserId);

        var created = await postRepo.AddAsync(newPost);

        var dto = new PostDto
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };

        return Created($"/posts/{dto.Id}", dto);
    }

    // GET /posts
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        var posts = postRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
            posts = posts.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));

        if (userId.HasValue)
            posts = posts.Where(p => p.UserId == userId);

        if (!string.IsNullOrWhiteSpace(username))
        {
            var matchingUserIds = userRepo.GetManyAsync()
                .Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToHashSet();

            posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
        }

        var dtos = posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            UserId = p.UserId
        }).ToList();

        return Ok(dtos);
    }

    // GET /posts/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSingle(int id)
    {
        try
        {
            var p = await postRepo.GetSingleAsync(id);
            return Ok(new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId
            });
        }
        catch
        {
            return NotFound();
        }
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdatePostDto request)
    {
        try
        {
            var existing = await postRepo.GetSingleAsync(id);

            // Create NEW Post instance with updated values
            var updated = new Post(request.Title, request.Body, existing.UserId);

            // keep ID the same using reflection
            typeof(Post).GetProperty("Id")!.SetValue(updated, existing.Id);

            await postRepo.UpdateAsync(updated);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }

    // DELETE /posts/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }
}
