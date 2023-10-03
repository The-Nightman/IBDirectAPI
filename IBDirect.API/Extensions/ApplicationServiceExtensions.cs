using IBDirect.API.Data;
using IBDirect.API.Interfaces;
using IBDirect.API.Services;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
