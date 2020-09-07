namespace Stateless.Web.Application.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Application;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/weatherforecasts")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator mediator;

        public WeatherForecastController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecastQueryResponse>> Get()
        {
            return await this.mediator.Send(new WeatherForecastsQuery()).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("{daysOffset:int}")]
        public async Task<IEnumerable<WeatherForecastQueryResponse>> Get(int daysOffset)
        {
            return await this.mediator.Send(
                new WeatherForecastsQuery(daysOffset)).ConfigureAwait(false);
        }
    }
}
