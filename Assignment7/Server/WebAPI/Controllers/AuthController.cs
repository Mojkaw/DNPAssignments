using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using ApiContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username
        };

        return Ok(userDto);
    }
}