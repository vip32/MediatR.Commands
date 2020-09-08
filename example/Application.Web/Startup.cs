namespace WeatherForecast.Application.Web
{
    using System.Net;
    using System.Threading.Tasks;
    using global::Application;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidateBehavior<,>));

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

                // controller commands/queries (/api/users)
                endpoints.MapControllers();

                // controllerless commands/queries (/users)
                endpoints.MapGet<UserFindAllQuery>(
                    "/users", "User");

                endpoints.MapGet<UserFindByIdQuery>(
                    "/users/{userId}", "User");

                endpoints.MapPost<UserCreateCommand>(
                    "/users",
                    response: new CommandEndpointResponse<UserCreateCommand, UserCreateCommandResponse>(
                        onSuccess: (req, res, ctx) => ctx.Response.Location($"/users/{res.UserId}"),
                        onSuccessStatusCode: HttpStatusCode.Created,
                        ignoreResponseBody: true),
                    openApi: new OpenApiDetails("User")
                    {
                        RequestBodyDescription = "The user details"
                    });

                endpoints.MapPut<UserUpdateCommand>(
                    "/users/{userId}", "User");
            });
        }
    }
}
