using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.API.Weather
{
    public interface IWeatherForecast
    {
        Task<dynamic> DoForecast( string model );
    }
}
