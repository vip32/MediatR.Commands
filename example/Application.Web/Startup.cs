namespace WeatherForecast.Application.Web
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::Application;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NSwag.Generation.Processors;

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

            services.AddSwaggerDocument((c, sp) =>
                sp.GetServices<IDocumentProcessor>()?.ForEach(dp => c.DocumentProcessors.Add(dp)));

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

            var a = app.ApplicationServices.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });

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
                endpoints.MapGet<WeatherForecastsQuery>(
                    "/reg/weatherforecasts/minimal",
                    openApi: new OpenApiDetails
                    {
                        GroupName = "test group",
                        Summary = "test summary 0"
                    }); // --> send to WeatherForecastsQueryHandler

                endpoints.MapGet<WeatherForecastsQuery>(
                    pattern: "/reg/weatherforecasts",
                    response: new CommandEndpointResponse<WeatherForecastsQuery, IEnumerable<WeatherForecastQueryResponse>>(
                        onSuccess: (req, res, ctx) => ctx.Response.Location($"api/customers/{req.QueryId}/{res.Count()}"),
                        onSuccessStatusCode: HttpStatusCode.OK),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "test group",
                        Summary = "test summary 1"
                    });

                endpoints.MapGet<WeatherForecastsQuery>(
                    pattern: "/reg/weatherforecasts/{DaysOffset:int}",
                    response: new CommandEndpointResponse(
                        onSuccess: (req, res, ctx) => ctx.Response.Location("api/customers"),
                        onSuccessStatusCode: HttpStatusCode.OK),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "test group",
                        Summary = "test summary 2"
                    });

                endpoints.MapPost<DoItCommand>(
                    pattern: "/reg/doit", // leading slash is mandatory!
                    response: new CommandEndpointResponse(
                        onSuccess: (req, res, ctx) => ctx.Response.Location("api/doit"),
                        onSuccessStatusCode: HttpStatusCode.Created),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "test group",
                        Summary = "test summary 3"
                    });

                endpoints.MapPost<DoItCommand>(
                    pattern: "/reg/doit2", // leading slash is mandatory!
                    response: new CommandEndpointResponse<DoItCommand>(
                        onSuccess: (req, ctx) => ctx.Response.Location($"api/doit/{req.FirstName}_{req.LastName}"),
                        onSuccessStatusCode: HttpStatusCode.Created),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "test group",
                        Summary = "test summary 4"
                    });
            });
        }
    }
}
