using Formfakten.TestLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NDO.Application;

namespace NdoUnitTests
{
	public class NDOTest
	{
		IHost host;
		public NDOTest()
		{
			var builder = Host.CreateDefaultBuilder();
			builder.ConfigureServices( services =>
			{
				services.AddLogging( b =>
				{
					b.ClearProviders();
					b.AddFormfaktenLogger();
				} );

				services.AddNdo( null, null );
			} );

			this.host = builder.Build();
			this.host.Services.UseNdo();
		}
	}
}
