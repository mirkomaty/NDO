using Formfakten.TestLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NDO.Application;
using H = Microsoft.Extensions.Hosting;

namespace NdoUnitTests
{
	public class NDOTest
	{
		public IHost Host { get; private set; }
		public NDOTest()
		{
			var builder = H.Host.CreateDefaultBuilder();
			builder.ConfigureServices( services =>
			{
				services.AddLogging( b =>
				{
					b.ClearProviders();
					b.AddFormfaktenLogger();
				} );

				services.AddNdo( null, null );
			} );

			this.Host = builder.Build();
			this.Host.Services.UseNdo();
		}
	}
}
