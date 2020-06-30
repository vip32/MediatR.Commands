namespace Application
{
    using System;
    using System.Collections.Generic;
    using MediatR.Commands;

    public class WeatherForecastsQuery : QueryBase<IEnumerable<WeatherForecastQueryResponse>>, ICachedQuery
    {
        public string CacheKey => "all_weatherforecasts";

        public TimeSpan? SlidingExpiration => new TimeSpan(0, 0, 15);

        public DateTimeOffset? AbsoluteExpiration => null;
    }
}
