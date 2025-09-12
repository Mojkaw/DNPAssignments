using RepositoryContracts;
namespace CLI.ManagePosts;

public sealed class ListPostView
{
    private readonly IPostRepository postRepository;
    
    public ListPostView(IPostRepository postRepository)
        {
        this.postRepository = postRepository;
        }

    public async Task ShowAsync()
    {
        var posts = postRepository.GetManyAsync().ToList();
        
        if (!posts.Any())
        {
            Console.WriteLine("No posts found");
            return;
        }
        
        Console.WriteLine("Id | Title");
        foreach (var p in posts)
            Console.WriteLine($"{p.Id} | {p.Title}");
        
        await Task.CompletedTask;
            
        
    }
}