using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new();

    public Task<User> AddAsync(User user)
    {
        int newId = users.Any() ? users.Max(u => u.Id) + 1 : 1;

        // Set ID via reflection (private setter)
        typeof(User).GetProperty("Id")!.SetValue(user, newId);

        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var existing = users.SingleOrDefault(u => u.Id == user.Id)
                       ?? throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        users.Remove(existing);
        users.Add(user);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var toRemove = users.SingleOrDefault(u => u.Id == id)
                       ?? throw new InvalidOperationException($"User with ID '{id}' not found");

        users.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        var user = users.SingleOrDefault(u => u.Id == id)
                   ?? throw new InvalidOperationException($"User with ID '{id}' not found");

        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public IQueryable<User> GetManyAsync() => users.AsQueryable();
}