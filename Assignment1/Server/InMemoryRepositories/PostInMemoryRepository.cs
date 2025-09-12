using Entities;
using RepositoryContracts;
namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private List<Post> posts = new();
    
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max (p => p.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault (p => p.Id == post.Id);
        if (existingPost == null)
        {
            throw new InvalidOperationException($"Post {post.Id} not found");
        }
        posts.Remove (existingPost);
        posts.Add (post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault (p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post {id} not found");
        }
        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        //Do implementation
        Post? post = posts.SingleOrDefault (p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post {id} not found");
        }
        
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return posts.AsQueryable();
    }
}