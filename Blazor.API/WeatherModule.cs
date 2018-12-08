using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blazor.API.Weather;
using Nancy;
using Nancy.ModelBinding;

namespace Blazor.API
{
    public class WeatherModule : NancyModule
    {
        /// <summary>
        /// Weather forecast implementation
        /// </summary>
        private IWeatherForecast _weatherForecast;

        /// <summary>
        /// C'tor
        /// </summary>
        public WeatherModule( IWeatherForecast weatherForecast ) : base( "/weather" )
        {
            if ( weatherForecast == null )
                throw new ArgumentNullException( "weatherForecast" );

            _weatherForecast = weatherForecast;

            Post( "/", async ( args, token) =>
            {
                WeatherForecastModel model = this.Bind<WeatherForecastModel>();

                if ( null != model )
                    return await _weatherForecast.DoForecast( model );
                else
                    return new Response() { StatusCode = HttpStatusCode.BadRequest };
            } );
        }
    }
}
