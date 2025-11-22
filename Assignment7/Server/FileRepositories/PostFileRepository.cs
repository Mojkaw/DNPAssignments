using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
            File.WriteAllText(filePath, "[]");
    }

    // Helper: create Post from primitive data
    private static Post CreatePostFromRaw(int id, int userId, string title, string body)
    {
        // create using the public constructor
        var post = new Post(title, body, userId);

        // assign ID using reflection (setter is private)
        typeof(Post).GetProperty("Id")!.SetValue(post, id);

        return post;
    }

    // Load file by reading as primitive dictionary
    private List<Post> Load()
    {
        var json = File.ReadAllText(filePath);

        var rawData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json)
                      ?? new List<Dictionary<string, JsonElement>>();

        var posts = new List<Post>();

        foreach (var raw in rawData)
        {
            int id = raw["Id"].GetInt32();
            int userId = raw["UserId"].GetInt32();
            string title = raw["Title"].GetString()!;
            string body = raw["Body"].GetString()!;

            posts.Add(CreatePostFromRaw(id, userId, title, body));
        }

        return posts;
    }

    // Save file as primitive objects
    private void Save(List<Post> posts)
    {
        var raw = posts.Select(p => new
        {
            p.Id,
            p.UserId,
            p.Title,
            p.Body
        });

        var json = JsonSerializer.Serialize(raw);
        File.WriteAllText(filePath, json);
    }

    public async Task<Post> AddAsync(Post post)
    {
        var posts = Load();

        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;

        // assign ID via reflection (private setter)
        typeof(Post).GetProperty("Id")!.SetValue(post, maxId + 1);

        posts.Add(post);
        Save(posts);

        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        var posts = Load();

        int index = posts.FindIndex(p => p.Id == post.Id);
        if (index < 0)
            throw new InvalidOperationException($"Post {post.Id} not found");

        posts[index] = post;
        Save(posts);
    }

    public async Task DeleteAsync(int id)
    {
        var posts = Load();

        var existing = posts.SingleOrDefault(p => p.Id == id)
                       ?? throw new InvalidOperationException($"Post {id} not found");

        posts.Remove(existing);
        Save(posts);
    }

    public async Task<Post?> GetSingleAsync(int id)
    {
        var posts = Load();

        return posts.SingleOrDefault(p => p.Id == id)
               ?? throw new InvalidOperationException($"Post {id} not found");
    }

    public IQueryable<Post> GetManyAsync()
    {
        return Load().AsQueryable();
    }
}
