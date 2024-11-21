using EpiTrello.Core.Models;

namespace EpiTrello.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class EpiTrelloContext : DbContext
{
    public DbSet<Board> Boards { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Stage> Stages { get; set; }
    
    public DbSet<Block> Blocks { get; set; }

    public EpiTrelloContext(DbContextOptions<EpiTrelloContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>()
            .HasIndex(u => u.Id)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Id)
            .IsUnique();
        
        modelBuilder.Entity<Stage>()
            .HasIndex(u => u.Id)
            .IsUnique();
        
        modelBuilder.Entity<Block>()
            .HasIndex(u => u.Id)
            .IsUnique();
    }
}
