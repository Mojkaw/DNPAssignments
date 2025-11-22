using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
            File.WriteAllText(filePath, "[]");
    }

    private static Comment CreateCommentFromRaw(int id, int userId, int postId, string body)
    {
        // Create using the public constructor
        var comment = new Comment(body, userId, postId);

        // Set ID using reflection (since setter is private)
        typeof(Comment).GetProperty("Id")!.SetValue(comment, id);

        return comment;
    }

    private List<Comment> Load()
    {
        var json = File.ReadAllText(filePath);

        var rawData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json)
                      ?? new List<Dictionary<string, JsonElement>>();

        var comments = new List<Comment>();

        foreach (var raw in rawData)
        {
            int id = raw["Id"].GetInt32();
            int userId = raw["UserId"].GetInt32();
            int postId = raw["PostId"].GetInt32();
            string body = raw["Body"].GetString()!;

            var comment = new Comment(body, userId, postId);
            typeof(Comment).GetProperty("Id")!.SetValue(comment, id);

            comments.Add(comment);
        }

        return comments;
    }


    private void Save(List<Comment> comments)
    {
        // We serialize only primitive properties
        var raw = comments.Select(c => new
        {
            c.Id,
            c.UserId,
            c.PostId,
            c.Body
        });

        var json = JsonSerializer.Serialize(raw);
        File.WriteAllText(filePath, json);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        var comments = Load();
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 0;

        typeof(Comment).GetProperty("Id")!.SetValue(comment, maxId + 1);

        comments.Add(comment);
        Save(comments);

        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        var comments = Load();
        int index = comments.FindIndex(c => c.Id == comment.Id);

        if (index < 0)
            throw new Exception($"Comment with ID '{comment.Id}' not found");

        comments[index] = comment;
        Save(comments);
    }

    public async Task DeleteAsync(int id)
    {
        var comments = Load();
        var existing = comments.SingleOrDefault(c => c.Id == id)
            ?? throw new InvalidOperationException($"Comment with ID '{id}' not found");

        comments.Remove(existing);
        Save(comments);
    }

    public async Task<Comment?> GetSingleAsync(int id)
    {
        var comments = Load();
        return comments.SingleOrDefault(c => c.Id == id)
            ?? throw new InvalidOperationException($"Comment with ID '{id}' not found");
    }

    public IQueryable<Comment> GetManyAsync()
    {
        return Load().AsQueryable();
    }
}
