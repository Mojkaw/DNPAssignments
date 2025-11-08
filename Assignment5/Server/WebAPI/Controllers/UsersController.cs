using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
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
        // Uniqueness check (case-insensitive)
        var exists = userRepo.GetManyAsync()
            .Any(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));
        if (exists) return Conflict($"Username '{request.Username}' is already in use.");

        var created = await userRepo.AddAsync(new User
        {
            Username = request.Username,
            Password = request.Password
        });

        var dto = new UserDto { Id = created.Id, Username = created.Username };
        return Created($"/users/{dto.Id}", dto);
    }

    // GET /users?usernameContains=abc
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany([FromQuery] string? usernameContains)
    {
        var q = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(usernameContains))
            q = q.Where(u => u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));

        var dtos = q.Select(u => new UserDto { Id = u.Id, Username = u.Username }).ToList();
        return Ok(dtos);
    }

    // GET /users/5
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
            return NotFound(); // repo throws when not found
        }
    }

    // PUT /users/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            var user = await userRepo.GetSingleAsync(id);
            user.Username = request.Username;
            user.Password = request.Password;
            await userRepo.UpdateAsync(user);
            return NoContent();
        }
        catch
        {
            return NotFound();
        }
    }

    // DELETE /users/5
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
