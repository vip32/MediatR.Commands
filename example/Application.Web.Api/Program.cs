namespace Application.Web.Aapi
{
    using System.IO;
    using Application.Web.Api;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public static class Program
    {
        public static readonly string AppName = typeof(Program).Namespace;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((context, builder) =>
               {
                   builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{context?.HostingEnvironment?.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
               })
               .UseSerilog((context, builder) =>
               {
                    builder
                      .ReadFrom.Configuration(context.Configuration)
                      .MinimumLevel.Verbose()
                      .Enrich.WithProperty("ServiceName", AppName)
                      .Enrich.FromLogContext()
                      .WriteTo.Trace()
                      .WriteTo.Console();
               })
               .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
