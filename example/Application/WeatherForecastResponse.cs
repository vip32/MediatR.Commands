namespace Application
{
    using System;

    public class WeatherForecastResponse
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
