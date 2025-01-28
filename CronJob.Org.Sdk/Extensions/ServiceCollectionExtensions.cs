using CronJob.Org.Sdk.Services.Interfaces;
using CronJob.Org.Sdk.Services.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace CronJob.Org.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronJobOrgSdk(this IServiceCollection services)
    {
        services.AddScoped<ICronJobService, CronJobService>();
        return services;
    }
}