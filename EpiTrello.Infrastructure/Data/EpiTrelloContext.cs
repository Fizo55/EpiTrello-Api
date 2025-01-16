using EpiTrello.Core.Models;

namespace EpiTrello.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class EpiTrelloContext : DbContext
{
    public DbSet<Board> Boards { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Stage> Stages { get; set; }
    
    public DbSet<Block> Blocks { get; set; }
    
    public DbSet<Ticket> Tickets { get; set; }
    
    public DbSet<InviteLink> InviteLink { get; set; }
    
    public DbSet<Comment> Comment { get; set; }

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

        modelBuilder.Entity<Ticket>()
            .HasIndex(u => u.Id)
            .IsUnique();

        modelBuilder.Entity<InviteLink>()
            .HasIndex(u => u.Id)
            .IsUnique();
        
        modelBuilder.Entity<Comment>()
            .HasIndex(u => u.Id)
            .IsUnique();
        
        modelBuilder.Entity<Board>()
            .HasMany(b => b.Blocks)
            .WithOne(b => b.Board)
            .HasForeignKey(b => b.BoardId);
        
        modelBuilder.Entity<Board>()
            .HasMany(t => t.Stages)
            .WithOne(b => b.Board)
            .HasForeignKey(t => t.BoardId);
    }
}
