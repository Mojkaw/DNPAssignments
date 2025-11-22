using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcCommentRepository : ICommentRepository
{
    private readonly AppContext ctx;

    public EfcCommentRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        await ctx.Comments.AddAsync(comment);
        await ctx.SaveChangesAsync();
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        bool exists = await ctx.Comments.AnyAsync(c => c.Id == comment.Id);
        if (!exists)
            throw new InvalidOperationException($"Comment {comment.Id} not found");

        ctx.Comments.Update(comment);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await ctx.Comments.SingleOrDefaultAsync(c => c.Id == id);
        if (existing == null)
            throw new InvalidOperationException($"Comment {id} not found");

        ctx.Comments.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    public async Task<Comment?> GetSingleAsync(int id)
    {
        var comment = await ctx.Comments.SingleOrDefaultAsync(c => c.Id == id);
        if (comment == null)
            throw new InvalidOperationException($"Comment {id} not found");

        return comment;
    }

    public IQueryable<Comment> GetManyAsync()
    {
        return ctx.Comments.AsQueryable();
    }
}