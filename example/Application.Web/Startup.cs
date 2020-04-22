namespace WeatherForecast.Application.Web
{
    using global::Application;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IMemoryCache>(sp => new MemoryCache(new MemoryCacheOptions()));
            services.AddMediatR(new[] { typeof(WeatherForecastsQuery).Assembly });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DummyQueryBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MemoryCacheQueryBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidateCommandBehavior<,>));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
