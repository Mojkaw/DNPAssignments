namespace Entities;

public class Comment
{
    public int Id { get; private set; }
    public string Body { get; private set; } = string.Empty;

    public int UserId { get; private set; }
    public int PostId { get; private set; }

    // Navigation
    public User User { get; private set; }
    public Post Post { get; private set; }

    private Comment() {} // EF requirement

    public Comment(string body, int userId, int postId)
    {
        Body = body;
        UserId = userId;
        PostId = postId;
    }
}