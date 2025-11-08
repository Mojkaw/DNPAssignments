using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public sealed class DeleteCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    public DeleteCommentView(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Delete comment ===");

        var comments = commentRepository
            .GetManyAsync()
            .OrderBy(c => c.PostId)
            .ThenBy(c => c.Id)
            .ToList();

        if (!comments.Any())
        {
            Console.WriteLine("No comments to delete.");
            return;
        }

        foreach (var c in comments)
        {
            string author;
            try
            {
                var u = await userRepository.GetSingleAsync(c.UserId);
                author = u.Username;
            }
            catch
            {
                author = $"#{c.UserId}";
            }

            var preview = string.IsNullOrWhiteSpace(c.Body)
                ? ""
                : (c.Body.Length > 30 ? c.Body[..30] + "..." : c.Body);

            Console.WriteLine($"Post {c.PostId} - [Comment #{c.Id}] {author}: {preview}");
        }

        Console.Write("Enter Comment ID to delete (blank to cancel): ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        if (!int.TryParse(input, out int commentId) || commentId <= 0)
        {
            Console.WriteLine("Invalid comment id.");
            return;
        }

        Console.Write($"Are you sure you want to delete Comment #{commentId}? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        try
        {
            await commentRepository.DeleteAsync(commentId);
            Console.WriteLine("Comment deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete failed: {ex.Message}");
        }
    }
}