using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageUsers;

public sealed class CreateUserView
{
    private readonly IUserRepository userRepository;
    
    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        string username;
        do
        {
            Console.WriteLine("Please enter a username:");
            username = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(username))
                Console.WriteLine("Username is required.");
        } while (string.IsNullOrEmpty(username));
        
        string password;
        do
        {
            Console.WriteLine("Please enter a password:");
            password = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(password))
                Console.WriteLine("Password is required.");
        } while (string.IsNullOrEmpty(password));
        
        // Use constructor (required for EF-style entity)
        var user = new User(username, password);

        await userRepository.AddAsync(user);
        
        Console.WriteLine("User created.");
    }
}