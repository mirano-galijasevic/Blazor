using System;

using Nancy;

namespace Blazor.API
{
    public class Version : NancyModule
    {
        public Version()
        {
            Get("/", _ => "Blazor.API v. 1.0"); 
        }
    }
}
