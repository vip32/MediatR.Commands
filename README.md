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