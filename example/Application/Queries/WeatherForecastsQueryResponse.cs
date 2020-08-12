namespace Application
{
    using System;

    public class WeatherForecastsQueryResponse
    {
        public int Index { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
