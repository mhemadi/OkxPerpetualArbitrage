using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;

namespace OkxPerpetualArbitrage.OkxApi
{
    public static class ApiServiceRegistration
    {
        public static void ApiServicesRegistration(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<OkexApiSetting>(configuration.GetSection("OkexApiSetting"));
            service.AddScoped<IOkexApi, OKEXV5Api>();
            service.AddTransient<IOkxApiWrapper, OkxApiWrapper>();

        }
    }
}
