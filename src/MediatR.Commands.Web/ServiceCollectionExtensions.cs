namespace MediatR.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using NSwag.Generation.Processors;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandEndpoints(this IServiceCollection services)
        {
            services.AddSingleton<ICommandEndpointConfiguration, CommandEndpointConfiguration>();
            services.AddSingleton<IDocumentProcessor, CommandEndpointDocumentProcessor>();
            //services.AddSingleton<IApiDescriptionGroupCollectionProvider, CommandEndpointApiDescriptionGroupCollectionProvider>();
            //services.AddSingleton<IApiDescriptionGroupCollectionProvider, ApiDescriptionGroupCollectionProvider>();

            return services;
        }
    }
}
