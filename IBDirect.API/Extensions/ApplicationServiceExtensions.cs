using IBDirect.API.Data;
using IBDirect.API.Interfaces;
using IBDirect.API.Services;
using IBDirect.API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        return services;
    }
}
