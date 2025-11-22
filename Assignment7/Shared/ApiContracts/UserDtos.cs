namespace ApiContracts;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UpdateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
}