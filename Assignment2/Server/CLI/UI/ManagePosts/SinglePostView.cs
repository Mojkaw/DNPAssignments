using RepositoryContracts;

namespace CLI.ManagePosts;

public sealed class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    
    public SinglePostView(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
        {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        }

    public async Task ShowAsync()
    {
        int id;
        while (true)
        {
            Console.WriteLine("Enter Post Id:");
            if (int.TryParse(Console.ReadLine(), out id)) break;
            Console.WriteLine("Please enter a number:");
        }

        Entities.Post post;
        try
        {
            post = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Post not found");
            return;
        }

        Entities.User user = null!;
        try
        {
            user = await userRepository.GetSingleAsync(post.UserId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("User not found");
            return;
        }
        
        Console.WriteLine();
        Console.WriteLine($"[{post.Id}] {post.Title}");
        Console.WriteLine($"by {(user != null ? user.Username : $"#{post.UserId}")}");
        Console.WriteLine(post.Body);
        Console.WriteLine();

        var comments = commentRepository.GetManyAsync().Where(c => c.PostId == post.Id).ToList();
        if (!comments.Any())
        {
            Console.WriteLine("Comment not found");
            return;
        }
        
        Console.WriteLine("Comments:");
        foreach (var c in comments)
        {
            string authorName;
            try
            {
                var u = await userRepository.GetSingleAsync(c.UserId);
                authorName = u.Username;
            }
            catch (InvalidOperationException)
            {
                authorName = $"#{c.UserId}";
            }
            
            Console.WriteLine($" - {authorName}: {c.Body}");
        }
        await Task.CompletedTask;
    }
}