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
        var alice = await usersRepo.AddAsync(new User { Username = "alice", Password = "pw1" });
        var bob   = await usersRepo.AddAsync(new User { Username = "bob",   Password = "pw2" });

        // --- Users write posts ---
        var p1 = await postsRepo.AddAsync(new Post { UserId = alice.Id, Title = "Hello world", Body = "My first post!" });
        var p2 = await postsRepo.AddAsync(new Post { UserId = bob.Id,   Title = "A sunny day", Body = "It was great outside." });

        // --- Users comment on posts ---
        var c1 = await commentsRepo.AddAsync(new Comment { PostId = p1.Id, UserId = bob.Id,   Body = "Nice post!" });
        var c2 = await commentsRepo.AddAsync(new Comment { PostId = p1.Id, UserId = alice.Id, Body = "Thanks!" });
        var c3 = await commentsRepo.AddAsync(new Comment { PostId = p2.Id, UserId = alice.Id, Body = "Love it :)" });

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

        // --- Read a single post with its comments (join via FKs) ---
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
        await postsRepo.UpdateAsync(new Post
        {
            Id     = p2.Id,
            UserId = p2.UserId,
            Title  = p2.Title + " (edited)",
            Body   = p2.Body  + " Really enjoyed it."
        });

        Console.WriteLine("\nAfter edit:");
        foreach (var post in postsRepo.GetManyAsync())
            Console.WriteLine($"[{post.Id}] {post.Title}");

        // --- Delete a comment ---
        await commentsRepo.DeleteAsync(c1.Id);
        Console.WriteLine($"\nDeleted comment #{c1.Id}. Post #{p1.Id} now has {commentsRepo.GetManyAsync().Count(c => c.PostId == p1.Id)} comments.");

        // --- Error handling example (try getting a non-existent post) ---
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
