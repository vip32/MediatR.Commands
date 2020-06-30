namespace Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR.Commands;
    using Microsoft.Extensions.Logging;

    public class WeatherForecastsQueryHandler : QueryHandlerBase<WeatherForecastsQuery, IEnumerable<WeatherForecastQueryResponse>>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastsQueryHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<IEnumerable<WeatherForecastQueryResponse>> Process(WeatherForecastsQuery request, CancellationToken cancellationToken)
        {
            var rng = new Random();
            return Task.Run(() => Enumerable.Range(1, 5).Select(index => new WeatherForecastQueryResponse
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToList().AsEnumerable());
        }
    }
}
