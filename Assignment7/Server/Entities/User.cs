namespace Entities;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;

    // Navigation
    public ICollection<Post> Posts { get; private set; } = new List<Post>();
    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    private User() {} // Required by EF Core

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}