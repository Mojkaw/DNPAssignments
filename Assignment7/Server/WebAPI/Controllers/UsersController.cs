using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto request)
    {
        // async uniqueness check
        bool exists = await userRepo.GetManyAsync()
            .AnyAsync(u => EF.Functions.Like(u.Username, request.Username));

        if (exists)
            return Conflict($"Username '{request.Username}' is already in use.");

        var newUser = new User(request.Username, request.Password);

        var created = await userRepo.AddAsync(newUser);

        var dto = new UserDto
        {
            Id = created.Id,
            Username = created.Username
        };

        return Created($"/users/{dto.Id}", dto);
    }

    // GET /users?usernameContains=abc
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetMany([FromQuery] string? usernameContains)
    {
        var q = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            // EF Core compatible string search
            q = q.Where(u => EF.Functions.Like(u.Username, $"%{usernameContains}%"));
        }

        var dtos = await q
            .Select(u => new UserDto { Id = u.Id, Username = u.Username })
            .ToListAsync();

        return Ok(dtos);
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id)
    {
        try
        {
            var user = await userRepo.GetSingleAsync(id);
            return Ok(new UserDto { Id = user.Id, Username = user.Username });
        }
        catch
        {
            return NotFound();
        }
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            var existing = await userRepo.GetSingleAsync(id);

            var updated = new User(request.Username, request.Password);

            // apply existing ID via reflection
            typeof(User).GetProperty("Id")!.SetValue(updated, existing.Id);

            await userRepo.UpdateAsync(updated);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }

    // DELETE /users/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }
}
