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
            services.AddMemoryCache();

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
                        GroupName = "WeatherForecast"
                    });

                endpoints.MapGet<WeatherForecastsQuery>(
                    pattern: "/reg/weatherforecasts",
                    response: new CommandEndpointResponse<WeatherForecastsQuery, IEnumerable<WeatherForecastQueryResponse>>(
                        onSuccess: (req, res, ctx) => ctx.Response.Location($"/api/customers/{req.QueryId}/{res.Count()}"),
                        onSuccessStatusCode: HttpStatusCode.OK),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "WeatherForecast"
                    });

                endpoints.MapGet<WeatherForecastsQuery>(
                    pattern: "/reg/weatherforecasts/{daysOffset}", // swagger: param needs to be camelized, due to matching camelized model property
                    response: new CommandEndpointResponse(
                        onSuccess: (req, res, ctx) => ctx.Response.Location("/api/customers"),
                        onSuccessStatusCode: HttpStatusCode.OK),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "WeatherForecast"
                    });

                //endpoints.MapPost<CreateUserCommand>(
                //    pattern: "/users",
                //    response: new CommandEndpointResponse(
                //        onSuccess: (req, res, ctx) => ctx.Response.Location("/users"),
                //        onSuccessStatusCode: HttpStatusCode.Created),
                //    openApi: new OpenApiDetails
                //    {
                //        GroupName = "User"
                //    });

                endpoints.MapGet<UserFindAllQuery>(
                    "/users",
                    openApi: new OpenApiDetails
                    {
                        GroupName = "User"
                    });

                endpoints.MapGet<UserFindByIdQuery>(
                    "/users/{userId}",
                    openApi: new OpenApiDetails
                    {
                        GroupName = "User"
                    });

                endpoints.MapPost<UserCreateCommand>(
                    "/users",
                    response: new CommandEndpointResponse<UserCreateCommand, UserCreateCommandResponse>(
                        onSuccess: (req, res, ctx) => ctx.Response.Location($"/users/{res.UserId}"),
                        onSuccessStatusCode: HttpStatusCode.Created,
                        ignoreResponseBody: true),
                    openApi: new OpenApiDetails
                    {
                        GroupName = "User"
                    });

                endpoints.MapPut<UserUpdateCommand>(
                    "/users/{userId}",
                    openApi: new OpenApiDetails
                    {
                        GroupName = "User"
                    });
            });
        }
    }
}
