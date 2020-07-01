namespace Application
{
    using System;
    using System.Collections.Generic;
    using MediatR.Commands;

    public class WeatherForecastsQuery : QueryBase<IEnumerable<WeatherForecastQueryResponse>>, ICachedQuery
    {
        public WeatherForecastsQuery()
        {
        }

        public WeatherForecastsQuery(int daysOffset)
        {
            this.DaysOffset = daysOffset;
        }

        public int DaysOffset { get; } = 1;

        public string CacheKey => "all_weatherforecasts_" + this.DaysOffset;

        public TimeSpan? SlidingExpiration => new TimeSpan(0, 0, 15);

        public DateTimeOffset? AbsoluteExpiration => null;
    }
}
