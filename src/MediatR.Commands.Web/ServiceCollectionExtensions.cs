namespace MediatR.Commands
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandEndpoints(this IServiceCollection services)
        {
            services.AddSingleton<ICommandEndpointConfiguration, CommandEndpointConfiguration>();

            return services;
        }
    }
}
