using RepositoryContracts;

namespace CLI.ManagePosts
{
    public sealed class ManagePostView
    {
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly ICommentRepository commentRepository;

        public ManagePostView(IPostRepository posts, IUserRepository users, ICommentRepository comments)
        {
            postRepository = posts;
            userRepository = users;
            commentRepository = comments;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                PrintOptions();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await new CreatePostView(postRepository, userRepository).ShowAsync();
                        break;
                    case "2":
                        await new ListPostView(postRepository).ShowAsync();
                        break;
                    case "3":
                        await new SinglePostView(postRepository, userRepository, commentRepository).ShowAsync();
                        break;
                    case "4":
                        await new AddCommentView(commentRepository,
                            userRepository, postRepository).ShowAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }
            }
        }
        private static void PrintOptions()
        {
            Console.WriteLine();
            Console.WriteLine("=== Manage posts ===");
            Console.WriteLine("1) Create post");
            Console.WriteLine("2) Posts overview (title, id)");
            Console.WriteLine("3) View single post (with comments)");
            Console.WriteLine("4) Add comment to a post");
            Console.WriteLine("0) Back");
            Console.Write("> "); 
        }
    }
}