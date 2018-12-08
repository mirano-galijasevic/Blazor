using System;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

using Blazor.API.Weather;

namespace Blazor.API
{
    public class MyNancyBootstrapper : DefaultNancyBootstrapper
    {
        /// <summary>
        /// Override application startup
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pipelines"></param>
        protected override void ApplicationStartup( TinyIoCContainer container, IPipelines pipelines )
        {
            container.Register<IWeatherForecast, Forecast>().AsSingleton();

            pipelines.BeforeRequest += ( ctx ) =>
            {
                if ( !ctx.Request.Path.EndsWith( "weather" )
                    && ctx.Request.Path != "/"
                    && ctx.Request.Method != "OPTIONS" )
                {
                    // TODO: Check the authentication header or token here, and return 401 if not authorized
                }

                return null; // This means that the request will be allowed to pass on down the pipeline
            };

            base.ApplicationStartup( container, pipelines );
        }
    }
}