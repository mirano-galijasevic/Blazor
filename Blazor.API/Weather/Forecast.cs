using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using Nancy;
using Newtonsoft.Json;
using System.Net;

namespace Blazor.API.Weather
{
    public class Forecast : IWeatherForecast
    {
        /// <summary>
        /// API key
        /// </summary>
        /// <remarks>
        /// This should probably go to the configuration file
        /// </remarks>
        private const string _apiKey = "f71fe06a5368e3ba415e7ebe61ba16cd";

        /// <summary>
        /// Root of the web service
        /// </summary>
        /// <remarks>
        /// This should probably go to the configuration file
        /// </remarks>
        private const string _apiAddressRoot = "http://api.openweathermap.org/data/2.5/forecast";

        /// <summary>
        /// C'tor
        /// </summary>
        public Forecast() 
        {}

        /// <summary>
        /// Get weather forecast
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<dynamic> DoForecast( string city )
        {
            StringBuilder address = new StringBuilder( _apiAddressRoot );
            address.Append( $"?q={city}&APPID={_apiKey}&units=metric" );
            
            HttpWebRequest request = ( HttpWebRequest )WebRequest.Create( address.ToString() );
            request.Method = "GET";
            request.ContentType = "application/json";

            string result = null;

            try
            {
                var response = await request.GetResponseAsync();
                result = UnpackResult( ( HttpWebResponse )response );
            }
            catch ( Exception ex )
            {
                //TODO: log this...for now we only diplay it in the command windows
                Console.WriteLine( ex.ToString() );
            }

            return new Response
            {
                ContentType = "application/json",
                StatusCode = Nancy.HttpStatusCode.OK,
                Contents = _ =>
                {
                    using ( var writer = new System.IO.StreamWriter( _ ) )
                    {
                        writer.Write( result );
                    }
                }
            };
        }

        /// <summary>
        /// Unpack the response stream
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private string UnpackResult( HttpWebResponse response )
        {
            string result = null;

            try
            {
                Stream stream = response.GetResponseStream();

                if ( stream != null )
                {
                    StreamReader sr = new StreamReader( stream );

                    if ( sr != null )
                    {
                        result = sr.ReadToEnd();

                        sr.Close();
                        sr.Dispose();
                    }

                    stream.Close();
                    stream.Dispose();
                }

                response.Dispose();
            }
            catch ( Exception ex )
            {
                //TODO: log this...for now we only diplay it in the command windows
                Console.WriteLine( ex.ToString() );
            }

            return result;
        }
    }
}
