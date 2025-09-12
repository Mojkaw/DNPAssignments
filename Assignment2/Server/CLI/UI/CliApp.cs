using CLI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;
using CLI.ManagePosts;

namespace CLI.UI;

public sealed class CliApp
{
    private readonly IUserRepository usersRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentsRepository;

    public CliApp(IUserRepository usersRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.usersRepository = usersRepository;
        this.commentsRepository = commentRepository;
        this.postRepository = postRepository;;
        
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to the CLI app!");
            Console.WriteLine("1) Manage Users");
            Console.WriteLine("2) Manage Posts");
            Console.WriteLine("0) Exit");
            Console.WriteLine("> ");
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    await new ManageUsersView(usersRepository).ShowAsync();
                    break;
                case "2":
                    await new ManagePostView(postRepository, usersRepository, commentsRepository).ShowAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        }
    }
}