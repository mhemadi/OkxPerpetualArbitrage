using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Application.Services;
using OkxPerpetualArbitrage.Application.Validators.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using OkxPerpetualArbitrage.Application.Services.BackgroundTaskServices;
using OkxPerpetualArbitrage.Application.Services.Channels;
using MediatR;

namespace OkxPerpetualArbitrage.Application
{
    public static class ApplicationServiceRegistration
    {
        public static void ApplicationServicesRegistration(this IServiceCollection service, IConfiguration configuration)
        {
           
            service.AddAutoMapper(Assembly.GetExecutingAssembly());
            service.AddMediatR(Assembly.GetExecutingAssembly());
            service.AddTransient<IValidator<ClosePositionDto>, ClosePositionDtoValidator>();
            service.AddTransient<IValidator<OpenPositionDto>, OpenPositionDtoValidator>();
            service.AddScoped<IPositionOpenCloseValidationLogic, PositionOpenCloseValidationLogic>();
            service.AddScoped<IPositionOpenCloseChannelWriterLogic, PositionOpenCloseChannelWriterLogic>();

            service.Configure<GeneralSetting>(configuration.GetSection("GeneralSetting"));
            service.AddTransient<IFundsUpdater, FundsUpdater>();
            service.AddTransient<IPotentialPositionUpdater, PotentialPositionUpdater>();
            service.AddTransient<IPositionOpener, PositionOpener>();
            service.AddTransient<IPositionCloser, PositionCloser>();
            service.AddTransient<ICleaner, Cleaner>();
            service.AddScoped<IPositionOpenCloseLogic, PositionManagerLogic>();
            service.AddSingleton<IClosePositionProcessingChannel, ClosePositionProcessingChannel>();
            service.AddSingleton<IOpenPositionProcessingChannel, OpenPositionProcessingChannel>();
            service.AddScoped<IInProgressDemandLogic, InProgressDemandLogic>();
            service.AddScoped<IPositionChunckSizeCalculator, PositionChunckSizeCalculator>();
            service.AddScoped<ITotalAvailableCloseSizeCalculatorLogic, TotalAvailableCloseSizeCalculatorLogic>();
            service.AddScoped<IPositionOpenCloseValidationLogic, PositionOpenCloseValidationLogic>();

            service.AddScoped<IOrderCreateLogic, OrderCreateLogic>();
            service.AddScoped<IOrderFillCreateLogic, OrderFillCreateLogic>();
            service.AddScoped<IOrderStateCheckLogic, OrderStateCheckLogic>();
            service.AddScoped<IPositionCheckLogic, PositionCheckLogic>();
            service.AddScoped<IPositionCheckLogic, PositionCheckLogic>();
            service.AddScoped<IPositionChunkCreateLogic, PositionChunkCreateLogic>();
            service.AddScoped<IPositionCloseLogic, PositionCloseLogic>();
            service.AddScoped<IPositionOpenLogic, PositionOpenLogic>();
            service.AddScoped<IResetPositionLogic, ResetPositionLogic>();
            service.AddScoped<IGetCurrentPositionsLogic, GetCurrentPositionsLogic>();
            service.AddScoped<IPotentialPositionProcessorLogic, PotentialPositionProcessorLogic>();
            service.AddScoped<IOkxApiLogic, OkxApiLogic>();
            service.AddScoped<IGetPotentialPositionsLogic, GetPotentialPositionsLogic>();
            service.AddScoped<ICancelDemandLogic, CancelDemandLogic>();
            service.AddScoped<IGetInProgressDemandsLogic, GetInProgressDemandsLogic>();
        }
    }
}
