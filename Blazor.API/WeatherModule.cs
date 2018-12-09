using System;
using System.Collections.Generic;
using System.Linq;
using Blazor.API.Weather;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

                       // Delete from here....
                       string data = null;
                       using ( var stream = new System.IO.MemoryStream() )
                       {
                           ( ( Response )result ).Contents.Invoke( stream );
                           data = System.Text.Encoding.UTF8.GetString( stream.ToArray() );
                       }

                       List<WeatherForecastData> weatherForecastData = new List<WeatherForecastData>();

                       var json = Newtonsoft.Json.Linq.JObject.Parse( data );
                       foreach ( JProperty item in ( JToken )json )
                       {
                           if ( item.Name == "list" )
                           {
                               JArray array = JArray.Parse( item.Value.ToString() );

                               foreach ( var iter in array.Children() )
                               {
                                   var itemProperties = iter.Children<JProperty>();

                                   var elemDate = itemProperties.FirstOrDefault( x => x.Name == "dt_txt" );
                                   var valDate = elemDate.Value;

                                   var elemMain = itemProperties.FirstOrDefault( x => x.Name == "main" );
                                   var valMain = elemMain.Value;

                                   WeatherForecastData newWeatherForecastData = new WeatherForecastData();
                                   newWeatherForecastData.Date = valDate.ToString();

                                   var itemMainProperties = valMain.Children<JProperty>();
                                   var elemTemp = itemMainProperties.FirstOrDefault( x => x.Name == "temp" );
                                   var valTemp = elemTemp.Value;

                                   newWeatherForecastData.Temp = valTemp.ToString();

                                   weatherForecastData.Add( newWeatherForecastData );
                               }

                               break;
                           }
                       }
                       // ...to here

                       return result;
                   }
                   else
                       return new Response() { StatusCode = HttpStatusCode.BadRequest };
               } );
        }
    }

    public class WeatherForecastData
    {
        public string Temp { get; set; }
        public string Date { get; set; }
    }
}
