using RepositoryContracts;
using Entities;
namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    
    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
        {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        }

    public async Task ShowAsync()
    {
        string title;
        do
        {
            Console.WriteLine("Enter title:");
            title = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(title)) Console.WriteLine("Title is required.");
        } while (string.IsNullOrEmpty(title));
        
        string body;
        do
        {
            Console.WriteLine("Enter body:");
            body = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(body)) Console.WriteLine("Body is required.");
        } while (string.IsNullOrEmpty(body));
        
        int userId;
        while (true)
        {
            Console.WriteLine("Author user id: ");
            if (int.TryParse(Console.ReadLine(), out userId)) break;
            Console.WriteLine("Please enter a number.");
        }

        try
        {
            var s = await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("User not found.");
            return;
        }
        
        var post = new Post {Title = title,  Body = body, UserId = userId};
        await postRepository.AddAsync(post);
        
        Console.WriteLine("Post created.");
    }
}