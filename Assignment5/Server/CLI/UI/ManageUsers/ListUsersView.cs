using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public sealed class ListUsersView
{
    private readonly IUserRepository userRepository;
    
    public ListUsersView(IUserRepository userRepository)
        {
        this.userRepository = userRepository;
        }

    public async Task ShowAsync()
    {
        var query = userRepository.GetManyAsync();
        var users = query.ToList();
        if (!users.Any())
            {
            Console.WriteLine("No users found.");
            return;
            }
        
        Console.WriteLine("Id | Username");
        foreach (var u in users)
            Console.WriteLine($"{u.Id} | {u.Username}");
        
        await Task.CompletedTask;
        
    }
}