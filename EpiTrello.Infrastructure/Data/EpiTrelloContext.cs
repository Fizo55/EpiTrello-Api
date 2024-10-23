using EpiTrello.Core.Models;

namespace EpiTrello.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class EpiTrelloContext : DbContext
{
    public DbSet<Board> Boards { get; set; }

    public EpiTrelloContext(DbContextOptions<EpiTrelloContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>()
            .HasIndex(u => u.Id)
            .IsUnique();
    }
}
