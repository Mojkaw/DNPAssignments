using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private List<Post> posts = new();
    
    public Task<Post> AddAsync(Post post)
    {
        int newId = posts.Any()
            ? posts.Max(p => p.Id) + 1
            : 1;

        // Set ID via reflection (private setter)
        typeof(Post).GetProperty("Id")!.SetValue(post, newId);

        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        var existingPost = posts.SingleOrDefault(p => p.Id == post.Id)
                           ?? throw new InvalidOperationException($"Post {post.Id} not found");

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var postToRemove = posts.SingleOrDefault(p => p.Id == id)
                           ?? throw new InvalidOperationException($"Post {id} not found");

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        var post = posts.SingleOrDefault(p => p.Id == id)
                   ?? throw new InvalidOperationException($"Post {id} not found");

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return posts.AsQueryable();
    }
}