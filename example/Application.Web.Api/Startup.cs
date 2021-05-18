namespace Application.Web.Api
{
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
            services.AddMediatR(new[] { typeof(UserFindAllQuery).Assembly });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DummyQueryBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MemoryCacheQueryBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MemoryCacheInvalidateCommandBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidateBehavior<,>));

            services.AddCommandEndpoints();
            services.AddMemoryCache();

            services.AddSwaggerDocument((c, sp) =>
                sp.GetServices<IDocumentProcessor>()?.ForEach(dp => c.DocumentProcessors.Add(dp)));

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

                // endpoint commands/queries (/users)
                endpoints.MapGet<UserFindAllQuery>(
                    "/users", "User");

                endpoints.MapGet<UserFindByIdQuery>(
                    "/users/{id}", "User");

                endpoints.MapPost<UserCreateCommand>(
                    "/users",
                    response: new CommandEndpointResponse<UserCreateCommand, UserCreateCommandResponse>(
                        onSuccess: (req, res, ctx) => ctx.Response.Location($"/users/{res.UserId}"),
                        onSuccessStatusCode: HttpStatusCode.Created,
                        ignoreResponseBody: true),
                    openApi: new OpenApiOperation("User")
                    {
                        RequestBodyDescription = "The user details"
                    });

                endpoints.MapPut<UserUpdateCommand>(
                    "/users/{id}", "User");
            });

            this.SeedCache(app);
        }

        private void SeedCache(IApplicationBuilder app)
        {
            // seed the cache with test data.
            var cache = app.ApplicationServices.GetRequiredService<IMemoryCache>();
            var user1 = new User { FirstName = "John", LastName = "Doe01", Id = "aaa" };
            var user2 = new User { FirstName = "John", LastName = "Doe02", Id = "bbb" };
            cache.Set($"users_{user1.Id}", user1);
            cache.Set($"users_{user2.Id}", user2);
        }
    }
}