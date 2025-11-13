
using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts
{
    public sealed class AddCommentView
    {
        private readonly ICommentRepository commentRepository;
        private readonly IUserRepository userRepository;
        private readonly IPostRepository postRepository;

        public AddCommentView(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository)
        {
            this.commentRepository = commentRepository;
            this.userRepository = userRepository;
            this.postRepository = postRepository;
        }

        public async Task ShowAsync()
        {
            int postId;
            while (true)
            {
                Console.Write("Post id: ");
                if (int.TryParse(Console.ReadLine(), out postId)) break;
                Console.WriteLine("Please enter a number.");
            }

            try
            {
                _ = await postRepository.GetSingleAsync(postId);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Post not found.");
                return;
            }
            
            int userId;
            while (true)
            {
                Console.Write("User id: ");
                if (int.TryParse(Console.ReadLine(), out userId)) break;
                Console.WriteLine("Please enter a number.");
            }

            try
            {
                _ = await userRepository.GetSingleAsync(userId);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("User not found.");
                throw;
            }

            string body;
            do
            {
                Console.Write("Comment: ");
                body = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(body)) Console.WriteLine("Comment body is required.");
            } while (string.IsNullOrEmpty(body));

            var comment = new Comment { Body = body, UserId = userId, PostId = postId };
            await commentRepository.AddAsync(comment);
            
            Console.WriteLine("Comment added.");
        }
    }
}
