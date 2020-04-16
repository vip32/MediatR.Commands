namespace Application
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using MediatR.Commands;

    public class WeatherForecastsQuery : IQuery<IEnumerable<WeatherForecastResponse>>, ICacheQuery
    {
        public string CacheKey => "all_weatherforecasts";

        public TimeSpan? SlidingExpiration => new TimeSpan(0, 0, 5);

        public DateTimeOffset? AbsoluteExpiration => null;
    }
}
