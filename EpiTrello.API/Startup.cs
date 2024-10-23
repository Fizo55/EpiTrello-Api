using EpiTrello.Infrastructure.Data;
using EpiTrello.Infrastructure.Factories;
using EpiTrello.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace EpiTrello.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<EpiTrelloContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<DaoFactory>();
        services.AddScoped<BoardService>();

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}