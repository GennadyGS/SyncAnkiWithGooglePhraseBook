using Microsoft.Extensions.DependencyInjection;
using UpdateAnki.Services;

namespace UpdateAnki;

internal static class ServiceConfigurator
{
    public static IServiceCollection RegisterServices(ServiceCollection services) =>
        services.AddScoped<UpdateAnkiServiceFactory>();
}
