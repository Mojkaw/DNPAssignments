using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
            File.WriteAllText(filePath, "[]");
    }

    // Create a User entity from primitive data
    private static User CreateUserFromRaw(int id, string username, string password)
    {
        // use the public constructor
        var user = new User(username, password);

        // set ID via reflection (private setter)
        typeof(User).GetProperty("Id")!.SetValue(user, id);

        return user;
    }

    // Load file manually (no DTO class)
    private List<User> Load()
    {
        var json = File.ReadAllText(filePath);

        var rawData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json)
                      ?? new List<Dictionary<string, JsonElement>>();

        var users = new List<User>();

        foreach (var raw in rawData)
        {
            int id = raw["Id"].GetInt32();
            string username = raw["Username"].GetString()!;
            string password = raw["Password"].GetString()!;

            var user = new User(username, password);

            // restore private ID
            typeof(User).GetProperty("Id")!.SetValue(user, id);

            users.Add(user);
        }

        return users;
    }


    // Save only simple properties
    private void Save(List<User> users)
    {
        var raw = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Password
        });

        var json = JsonSerializer.Serialize(raw);
        File.WriteAllText(filePath, json);
    }

    public async Task<User> AddAsync(User user)
    {
        var users = Load();

        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;

        // assign ID via reflection
        typeof(User).GetProperty("Id")!.SetValue(user, maxId + 1);

        users.Add(user);
        Save(users);

        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var users = Load();

        int index = users.FindIndex(u => u.Id == user.Id);
        if (index < 0)
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        users[index] = user;
        Save(users);
    }

    public async Task DeleteAsync(int id)
    {
        var users = Load();

        var existing = users.SingleOrDefault(u => u.Id == id)
                       ?? throw new InvalidOperationException($"User with ID '{id}' not found");

        users.Remove(existing);
        Save(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        var users = Load();

        return users.SingleOrDefault(u => u.Id == id)
               ?? throw new InvalidOperationException($"User with ID '{id}' not found");
    }

    public IQueryable<User> GetManyAsync()
    {
        return Load().AsQueryable();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var users = Load();

        return users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}
