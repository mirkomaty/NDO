using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace NDO.ProviderFactory
{
    internal class NDOApplication
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static IHostEnvironment HostEnvironment { get; set; }
    }
}
