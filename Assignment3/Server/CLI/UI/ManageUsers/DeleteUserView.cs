using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public sealed class DeleteUserView
{
    private readonly IUserRepository userRepository;

    public DeleteUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Delete user ===");

        var users = userRepository.GetManyAsync().ToList();
        if (!users.Any())
        {
            Console.WriteLine("No users to delete.");
            return;
        }

        foreach (var u in users)
            Console.WriteLine($"{u.Id}) {u.Username}");

        Console.Write("Enter user ID to delete (blank to cancel): ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write($"Are you sure you want to delete user {id}? (y/N): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine("User deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete failed: {ex.Message}");
        }
    }
}