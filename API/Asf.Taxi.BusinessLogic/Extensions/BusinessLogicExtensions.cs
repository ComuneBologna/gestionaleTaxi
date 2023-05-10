using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Services;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartTech.Common.ApplicationUsers;
using SmartTech.Common.ApplicationUsers.Entities;

namespace Asf.Taxi.BusinessLogic.Extensions
{
    public static class BusinessLogicExtensions
    {
        public static IServiceCollection AddTaxiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IUsersMapper, UsersMapper>();
            services.AddSingleton<IUserPermissions, TaxiPermissions>();

            services.AddUsersServices<TaxiDriverDBContext, UserEntity, TaxiAuthorityUserEntity>(Environment.GetEnvironmentVariable("TaxiConnectionString") ??
                configuration.GetSection("Taxi:ConnectionString")?.Value ?? string.Empty);

            services.AddScoped<ILicenseesService, LicenseesService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<ITaxiDriverAssociationsService, TaxiDriverAssociationsService>();
            services.AddScoped<IVehiclesService, VehiclesService>();
            services.AddScoped<ITaxiDriversService, TaxiDriversService>();
            services.AddScoped<IAuditsService, AuditsService>();
            //services.AddScoped<IImportService, ImportService>();
            services.AddScoped<IShiftsService, ShiftsService>();
            services.AddScoped<IFinancialAdministrationService, FinancialAdministrationService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IProtocolService, ProtocolService>();
            services.AddScoped<IDigitalSignService, DigitalSignService>();

            return services;
        }

        public static SubstitutionStatus CalculateSubstitutionStatus(this DateTime now, DateTime startDate, DateTime endDate)
            => now >= startDate && now <= endDate ? SubstitutionStatus.Active : now > endDate ? SubstitutionStatus.Terminated : SubstitutionStatus.ToActivate;

    }
}