using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new();

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any() ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        var existing = comments.SingleOrDefault(c => c.Id == comment.Id)
                       ?? throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");

        comments.Remove(existing);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var toRemove = comments.SingleOrDefault(c => c.Id == id)
                       ?? throw new InvalidOperationException($"Comment with ID '{id}' not found");

        comments.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        var comment = comments.SingleOrDefault(c => c.Id == id)
                      ?? throw new InvalidOperationException($"Comment with ID '{id}' not found");

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetManyAsync() => comments.AsQueryable();
}