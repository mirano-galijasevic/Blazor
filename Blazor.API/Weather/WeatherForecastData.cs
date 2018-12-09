using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.API.Weather
{
    public class WeatherForecastData
    {
        public string Temp { get; set; }

        public string Date { get; set; }

        public string Pressure { get; set; }

        public string Humidity { get; set; }

        public string Weather { get; set; }

        public string Clouds { get; set; }

        public string Wind { get; set; }

        public string Rain { get; set; }
    }
}
