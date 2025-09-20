using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public sealed class DeletePostView
{
    private readonly IPostRepository postRepository;

    public DeletePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Delete post ===");

        var posts = postRepository.GetManyAsync().ToList();
        if (!posts.Any())
        {
            Console.WriteLine("No posts to delete.");
            return;
        }

        foreach (var p in posts)
            Console.WriteLine($"{p.Id}) {p.Title}");

        Console.Write("Enter post ID to delete (blank to cancel): ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write($"Are you sure you want to delete post {id}? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        try
        {
            await postRepository.DeleteAsync(id);
            Console.WriteLine("Post deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete failed: {ex.Message}");
        }
    }
}