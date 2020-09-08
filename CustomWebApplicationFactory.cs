namespace IntegrationTests
{
    using Application;
    using Application.Web;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CustomWebApplicationFactory<TStartup> : aaaaaWebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .ConfigureServices(services =>
                {
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the cahce
                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

                        // Seed the cache with test data.
                        var user1 = new User { FirstName = "John", LastName = "Doe01", Id = "aaa" };
                        var user1 = new User { FirstName = "John", LastName = "Doe01", Id = "bbb" };
                        this.cache.Set($"users_{user1.Id}", user1);
                        this.cache.Set($"users_{user2.Id}", user2);
                    }
                });
        }
    }
}
