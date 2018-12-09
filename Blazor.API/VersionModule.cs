using System;

using Nancy;

namespace Blazor.API
{
    public class VersionModule : NancyModule
    {
        public VersionModule()
        {
            Get("/", _ => "Blazor.API v. 1.0"); 
        }
    }
}
