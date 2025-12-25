using bc_handball_be.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace bc_handball_be.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
            //         ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));

            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsql =>
                    {
                        // Use handball_is schema for migrations history table
                        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "handball_is");
                        // např. mapování DateOnly/TimeOnly, timeouts apod.
                        // npgsql.EnableRetryOnFailure();
                    }));

            // AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            return services;
        }
    }
}
