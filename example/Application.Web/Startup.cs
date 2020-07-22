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

                // command routes (registrations)
                endpoints.MapGet<WeatherForecastsQuery>("/reg/weatherforecasts/minimal");

                endpoints.MapGet<WeatherForecastsQuery>(
                    pattern: "/reg/weatherforecasts",
                    response: new CommandEndpointResponse<WeatherForecastsQuery, IEnumerable<WeatherForecastQueryResponse>>(
                        onSuccess: (req, res, ctx) => ctx.Response.Location($"api/customers/{req.QueryId}/{res.Count()}"),
                        onSuccessStatusCode: HttpStatusCode.OK),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "test",
                        Summary = "test"
                    });

                endpoints.MapGet<WeatherForecastsQuery>(
                    pattern: "/reg/weatherforecasts/{DaysOffset:int}",
                    response: new CommandEndpointResponse(
                        onSuccess: (req, res, ctx) => ctx.Response.Location("api/customers"),
                        onSuccessStatusCode: HttpStatusCode.OK));

                endpoints.MapPost<DoItCommand>(
                    pattern: "reg/doit",
                    response: new CommandEndpointResponse(
                        onSuccess: (req, res, ctx) => ctx.Response.Location("api/doit"),
                        onSuccessStatusCode: HttpStatusCode.Created));

                endpoints.MapPost<DoItCommand>(
                    pattern: "reg/doit2",
                    response: new CommandEndpointResponse<DoItCommand>(
                        onSuccess: (req, ctx) => ctx.Response.Location($"api/doit/{req.FirstName}_{req.LastName}"),
                        onSuccessStatusCode: HttpStatusCode.Created));
            });
        }
    }
}
