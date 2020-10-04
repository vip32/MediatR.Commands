[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vip32_MediatR.Commands&metric=alert_status)](https://sonarcloud.io/dashboard?id=vip32_MediatR.Commands)
[![NuGet](https://img.shields.io/nuget/v/MediatR.Commands.svg)](https://www.nuget.org/packages/MediatR.Commands/)

# MediatR.Commands
A simple commands & queries extension for the great [MediatR](https://github.com/jbogard/MediatR) library.

## Usage

Startup.cs::ConfigureServices()
```
    services.AddMediatR(new[] { typeof(WeatherForecastsQuery).Assembly });
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DummyQueryBehavior<,>));
```

Startup.cs::Configure()
```
TODO
```

Controller
```
    [ApiController]
    [Route("weatherforecasts")]
    public class WeatherForecastController : ControllerBase
    {
        ...

        [HttpGet]
        public async Task<IEnumerable<WeatherForecastResponse>> Get()
        {
            return await this.mediator.Send(new WeatherForecastsQuery()).ConfigureAwait(false);
        }
    }
```

or without Controllers [see Startup.cs](https://github.com/vip32/MediatR.Commands/blob/5202b64067d785828277c048ed87ad426d546ff8/example/Application.Web.Api/Startup.cs#L73-L77)
```
services.AddCommandEndpoints(); // ConfigureServices

app.UseEndpoints(endpoints => // Configure
{
  ... 

  endpoints.MapGet<UserFindAllQuery>(
    "/users", "User");

  endpoints.MapGet<UserFindByIdQuery>(
    "/users/{userId}", "User");

  ...
}
```
```