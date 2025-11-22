namespace EfcRepositories;
using Microsoft.EntityFrameworkCore;
using Entities;

public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            @"Data Source=C:\Users\hiayg\Desktop\DNPAssignments\Assignment7\Server\EfcRepositories\app.db");
    }
}