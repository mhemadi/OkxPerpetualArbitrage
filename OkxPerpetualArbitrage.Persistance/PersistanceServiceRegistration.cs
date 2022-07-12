using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Persistance.Repositories;

namespace OkxPerpetualArbitrage.Persistance
{
    public static class PersistanceServiceRegistration
    {
        public static void PersistanceServicesRegistration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<SpotPerpDbContext>(options => options.UseSqlite(configuration.GetConnectionString("sqliteConnectionString")));
            service.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
            service.AddScoped<IFundingIncomeRepository, FundingIncomeRepository>();
            service.AddScoped<IOrderFillRepository, OrderFillRepository>();
            service.AddScoped<IPositionDemandRepository, PositionDemandRepository>();
            service.AddScoped<IPotentialPositionRepository, PotentialPositionRepository>();
            service.AddScoped<IPotentialPositionRatingHistoryRepository, PotentialPositionRatingHistoryRepository>();
            service.AddScoped<IPositionHistoryRepository, PositionHistoryRepository>();
        }
    }
}
