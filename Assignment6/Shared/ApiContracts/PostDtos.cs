namespace ApiContracts;

public class CreatePostDto
{
    public required string Title { get; set; }
    public required string Body { get; set; }
    public int UserId { get; set; }
}

public class UpdatePostDto
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public int UserId { get; set; }
}