namespace IntegrationTests
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Application.Web.Api;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1#basic-tests-with-the-default-webapplicationfactory
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureTestServices(services => services
                        .AddAuthentication(options => // add a fake authentication handler
                        {
                            options.DefaultAuthenticateScheme = FakeAuthenticationHandler.SchemeName; // use the fake handler instead of the jwt handler (Startup)
                            options.DefaultScheme = FakeAuthenticationHandler.SchemeName;
                        })
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(FakeAuthenticationHandler.SchemeName, null));
                });
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
#pragma warning restore SA1402 // File may only contain a single type
    {
        public FakeAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        public static string SchemeName => "Fake";

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Email, "john.doe@test.com"),
                //new Claim("sub", "2020")
            };
            var identity = new ClaimsIdentity(claims, this.Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket)).ConfigureAwait(false);
        }
    }
}