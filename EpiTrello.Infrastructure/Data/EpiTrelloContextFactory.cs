using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EpiTrello.Infrastructure.Data;

public class EpiTrelloContextFactory : IDesignTimeDbContextFactory<EpiTrelloContext>
{
    public EpiTrelloContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<EpiTrelloContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new EpiTrelloContext(optionsBuilder.Options);
    }
}