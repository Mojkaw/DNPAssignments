using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcUserRepository : IUserRepository
{
    private readonly AppContext ctx;

    public EfcUserRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task<User> AddAsync(User user)
    {
        await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        bool exists = await ctx.Users.AnyAsync(u => u.Id == user.Id);
        if (!exists)
            throw new InvalidOperationException($"User {user.Id} not found");

        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await ctx.Users.SingleOrDefaultAsync(u => u.Id == id);
        if (existing == null)
            throw new InvalidOperationException($"User {id} not found");

        ctx.Users.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    public async Task<User?> GetSingleAsync(int id)
    {
        var user = await ctx.Users.SingleOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new InvalidOperationException($"User {id} not found");

        return user;
    }

    public IQueryable<User> GetManyAsync()
    {
        return ctx.Users.AsQueryable();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await ctx.Users.SingleOrDefaultAsync(
            u => u.Username.ToLower() == username.ToLower()
        );
    }
}