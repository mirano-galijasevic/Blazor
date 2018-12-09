using System;
using System.Collections.Generic;
using System.Linq;
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

            Get( "/{city}", async ( args, token ) =>
               {
                   var city = args[ "city" ];

                   if ( null != city )
                   {
                       var result = await _weatherForecast.DoForecast( city );
                       return result;
                   }
                   else
                       return new Response() { StatusCode = HttpStatusCode.BadRequest };
               } );
        }
    }
}
