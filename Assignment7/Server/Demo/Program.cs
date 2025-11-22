using Entities;
using InMemoryRepositories;

namespace Demo;

public class Program
{
    public static async Task Main(string[] args)
    {
        var usersRepo    = new UserInMemoryRepository();
        var postsRepo    = new PostInMemoryRepository();
        var commentsRepo = new CommentInMemoryRepository();

        // --- Users create accounts ---
        var alice = await usersRepo.AddAsync(new User("alice", "pw1"));
        var bob   = await usersRepo.AddAsync(new User("bob",   "pw2"));

        // --- Users write posts ---
        var p1 = await postsRepo.AddAsync(new Post("Hello world", "My first post!", alice.Id));
        var p2 = await postsRepo.AddAsync(new Post("A sunny day", "It was great outside.", bob.Id));

        // --- Users comment on posts ---
        var c1 = await commentsRepo.AddAsync(new Comment("Nice post!", bob.Id, p1.Id));
        var c2 = await commentsRepo.AddAsync(new Comment("Thanks!",    alice.Id, p1.Id));
        var c3 = await commentsRepo.AddAsync(new Comment("Love it :)", alice.Id, p2.Id));

        // --- List all posts with author and comment count ---
        Console.WriteLine("All posts:");
        var usersQ = usersRepo.GetManyAsync();
        var postsQ = postsRepo.GetManyAsync();
        var commQ  = commentsRepo.GetManyAsync();

        foreach (var post in postsQ)
        {
            var author = usersQ.First(u => u.Id == post.UserId).Username;
            var count  = commQ.Count(c => c.PostId == post.Id);
            Console.WriteLine($"[{post.Id}] {post.Title} by {author} ({count} comments)");
        }

        // --- Read a single post with its comments ---
        Console.WriteLine("\nDetails for first post:");
        var post1 = await postsRepo.GetSingleAsync(p1.Id);
        var post1Author = usersQ.First(u => u.Id == post1.UserId).Username;
        Console.WriteLine($"Post #{post1.Id} - {post1.Title} by {post1Author}\n{post1.Body}\nComments:");
        foreach (var cm in commQ.Where(c => c.PostId == post1.Id))
        {
            var commenter = usersQ.First(u => u.Id == cm.UserId).Username;
            Console.WriteLine($" - {commenter}: {cm.Body}");
        }

        // --- Update a post ---
        var postToUpdate = await postsRepo.GetSingleAsync(p2.Id);

        // Create NEW Post instance with updated text (private setters → cannot modify existing)
        var updated = new Post(
            postToUpdate.Title + " (edited)",
            postToUpdate.Body  + " Really enjoyed it.",
            postToUpdate.UserId
        );

        // Copy original ID using reflection because Id has a private setter
        typeof(Post).GetProperty("Id")!.SetValue(updated, postToUpdate.Id);

        await postsRepo.UpdateAsync(updated);

        Console.WriteLine("\nAfter edit:");
        foreach (var post in postsRepo.GetManyAsync())
            Console.WriteLine($"[{post.Id}] {post.Title}");

        // --- Delete a comment ---
        await commentsRepo.DeleteAsync(c1.Id);
        Console.WriteLine($"\nDeleted comment #{c1.Id}. Post #{p1.Id} now has {commentsRepo.GetManyAsync().Count(c => c.PostId == p1.Id)} comments.");

        // --- Error handling example ---
        try
        {
            await postsRepo.GetSingleAsync(999);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\nExpected error: {ex.Message}");
        }
    }
}
