using NDO;
using NDO.UISupport;
using SqlServerUISupport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TestUIProviders
{
	internal class NdoUIProviderFactory
	{
		private Dictionary<string, IDbUISupport> uiSupportProviders;
		private static object lockObject = new object();
		private NdoUIProviderFactory()
		{
		}

		public static NdoUIProviderFactory Instance = new NdoUIProviderFactory();
		public IDbUISupport this[string name]
		{
			get
			{
				if (this.uiSupportProviders == null)
				{
					FetchProviders();
				}

				IDbUISupport result;
				uiSupportProviders.TryGetValue( name, out result );
				return result;
			}
		}

		private void FetchProviders()
		{
			lock (lockObject)
			{
				if (this.uiSupportProviders == null) // double check
				{
					this.uiSupportProviders = new Dictionary<string, IDbUISupport>();
					string path = Path.GetDirectoryName( GetType().Assembly.Location );

					var ifc = typeof( IDbUISupport );

					foreach (var fileName in Directory.GetFiles( path, "*UISupport.dll" ))
					{
						if (fileName.EndsWith( "NDO.UISupport.dll" ))
							continue;

						try
						{
							var assembly = Assembly.LoadFrom( fileName );
							foreach (var type in assembly.ExportedTypes.Where( t => ifc.IsAssignableFrom( t ) ))
							{
								var p = (IDbUISupport)Activator.CreateInstance( type );
								this.uiSupportProviders.Add( p.Name, p );
								var provider = NDOProviderFactory.Instance[p.Name];
								if (provider == null)
									Debug.WriteLine( $"No NDO provider for UI provider {p.Name}" );
								else
									p.Initialize( provider );
							}
						}
						catch (Exception ex)
						{
							Debug.WriteLine( fileName + ": " + ex.ToString() );
						}
					}
				}
			}
		}

		public string[] Keys
		{
			get
			{
				if (this.uiSupportProviders == null)
					FetchProviders();

				return (from s in this.uiSupportProviders.Keys select s).ToArray();
			}
		}
	}
}
