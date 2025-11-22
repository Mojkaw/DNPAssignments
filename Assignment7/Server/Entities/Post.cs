namespace Entities;

public class Post
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;

    public int UserId { get; private set; } // Foreign key

    // Navigation
    public User User { get; private set; }
    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    private Post() {} // For EF

    public Post(string title, string body, int userId)
    {
        Title = title;
        Body = body;
        UserId = userId;
    }
}