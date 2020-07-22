namespace WeatherForecast.Application.Web
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using global::Application;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
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

            // commands registrations
            services.AddSingleton<IMemoryCache>(sp => new MemoryCache(new MemoryCacheOptions()));
            services.AddMediatR(new[] { typeof(WeatherForecastsQuery).Assembly });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DummyQueryBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MemoryCacheQueryBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidateCommandBehavior<,>));

            services.AddCommandEndpoints();

            // optional authentication
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //    options.Authority = "TODO";
            //    options.Audience = "TODO";
            //    options.TokenValidationParameters.ValidateLifetime = true;
            //    options.TokenValidationParameters.ClockSkew = System.TimeSpan.Zero;
            //});

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // command routes
                //endpoints.MapGet<WeatherForecastsQuery>("/api/weatherforecasts");
                //endpoints.MapGet<WeatherForecastsQuery>("/api/weatherforecasts/{DaysOffset:int}");

                // commands routes (without controllers)
                endpoints.MapGet("/api/weatherforecasts", async context =>
                {
                    var mediator = context.Request.HttpContext.RequestServices.GetRequiredService<IMediator>();
                    var response = await mediator.Send(new WeatherForecastsQuery()).ConfigureAwait(false);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
                });
                endpoints.MapGet("/api/weatherforecasts/{DaysOffset:int}", async context =>
                {
                    var mediator = context.Request.HttpContext.RequestServices.GetRequiredService<IMediator>();
                    var daysOffset = int.Parse((string)context.Request.RouteValues["DaysOffset"]);
                    var response = await mediator.Send(new WeatherForecastsQuery(daysOffset)).ConfigureAwait(false);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
                });

                endpoints.MapGet<WeatherForecastsQuery>(
                    "/api/weatherforecasts2",
                    new CommandEndpointResponseQuery<WeatherForecastsQuery, IEnumerable<WeatherForecastQueryResponse>>
                    {
                        OnSuccessStatusCode = HttpStatusCode.OK,
                        OnSuccess = (req, res, ctx) => ctx.Response.Location($"api/customers/{req.QueryId}/{res.Count()}") // typed req, res
                    },
                    new OpenApiDetails
                    {
                        GroupName = "test",
                        Summary = "test"
                    });
                endpoints.MapGet<WeatherForecastsQuery>(
                    "/api/weatherforecasts2/{DaysOffset:int}",
                    new CommandEndpointResponse
                    {
                        OnSuccessStatusCode = HttpStatusCode.OK,
                        OnSuccess = (req, res, ctx) => ctx.Response.Location("api/customers") // no req, res
                    });
            });
        }
    }
}
