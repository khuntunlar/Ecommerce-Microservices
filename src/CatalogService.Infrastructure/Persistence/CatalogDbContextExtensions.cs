using CatalogService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure.Persistence;

public static class CatalogDbContextExtensions
{
    public static IServiceCollection AddCatalogPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Catalog")
            ?? "Server=localhost;Port=3306;Database=catalog_service;User=root;Password=password;";

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());
        return services;
    }
}
