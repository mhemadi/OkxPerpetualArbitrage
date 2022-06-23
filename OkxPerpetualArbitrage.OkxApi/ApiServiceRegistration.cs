using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;

namespace OkxPerpetualArbitrage.OkxApi
{
    public static  class ApiServiceRegistration
    {
        public static void ApiServicesRegistration(this IServiceCollection service,  IConfiguration configuration)
        {
            service.Configure<OkexApiSetting>(configuration.GetSection("OkexApiSetting"));
            service.AddScoped<IOkexApi, OKEXV5Api>();
            service.AddTransient<IApiService, ApiService>();

        }
    }
}
