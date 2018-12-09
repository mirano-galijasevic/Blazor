using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blazor.API.Weather
{
    public class WeatherForecastResult
    {
        /// <summary>
        /// Web response result
        /// </summary>
        private HttpWebResponse _response;

        /// <summary>
        /// C'tor
        /// </summary>
        public WeatherForecastResult( HttpWebResponse response )
        {
            if ( response == null )
                throw new ArgumentNullException( "response" );

            _response = response;
        }

        /// <summary>
        /// Parse result
        /// </summary>
        /// <returns></returns>
        public string ParseResult()
        {
            string data = UnpackResult( ( HttpWebResponse )_response );

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

                        WeatherForecastData newWeatherForecastData = new WeatherForecastData();
                        newWeatherForecastData.Date = GetElement( itemProperties, "dt_txt" );

                        // Main
                        newWeatherForecastData.Temp = GetElement( itemProperties, "main", "temp" );
                        newWeatherForecastData.Pressure = GetElement( itemProperties, "main", "pressure" );
                        newWeatherForecastData.Humidity = GetElement( itemProperties, "main", "humidity" );

                        // Weather
                        newWeatherForecastData.Weather = GetElement( itemProperties, "weather", "description" );

                        // Clouds
                        newWeatherForecastData.Clouds = GetElement( itemProperties, "clouds", "all" );

                        // Wind
                        newWeatherForecastData.Wind = GetElement( itemProperties, "wind", "speed" );

                        // Rain
                        newWeatherForecastData.Rain = GetElement( itemProperties, "rain", "3h" );

                        weatherForecastData.Add( newWeatherForecastData );
                    }

                    break;
                }
            }

            var serializedData = JsonConvert.SerializeObject( weatherForecastData );
            return serializedData;
        }

        /// <summary>
        /// Get element from property
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private string GetElement( JEnumerable<JProperty> root, string elementName, string subElement = null )
        {
            string result = null;

            var elem = root.FirstOrDefault( x => x.Name == elementName );
            if ( elem == null )
                return String.Empty;

            result = elem.Value.ToString();
            
            if ( subElement != null )
            {
                if ( elementName == "weather" )
                {
                    JArray array = JArray.Parse( elem.Value.ToString() );
                    foreach( var item in array.Children())
                    {
                        var props = item.Children<JProperty>();
                        var resElem = props.FirstOrDefault( x => x.Name == subElement );
                        if ( resElem != null )
                            result = resElem.Value.ToString();
                        else
                            result = "0.0";

                        return result;
                    }
                }

                var itemProperties = elem.Value.Children<JProperty>();

                var subElem = itemProperties.FirstOrDefault( x => x.Name == subElement );
                if ( subElem != null )
                    result = subElem.Value.ToString();
                else
                    result = "0.0";

                return result;
            }

            return result;
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
