using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace EnhancerX86Stub
{
	class Program
	{
		/// <summary>
		/// This application launches the NDOEnhancer in x86 mode to enhance x86 assemblies.
		/// </summary>
		/// <param name="args"></param>
		public static int Main( string[] args )
		{
			int result = 0;

			try
			{
				if ( args.Length < 1 )
					throw new ArgumentException( "EnhancerX86Stub.exe needs at least one parameter" );
				string loadPath = typeof( Program ).Assembly.Location;
				if ( !File.Exists( loadPath ) )
					throw new Exception( "File not found: " + loadPath );
				loadPath = Path.Combine( Path.GetDirectoryName( loadPath ), "NDOEnhancer.exe" );
				result = AppDomain.CurrentDomain.ExecuteAssembly( loadPath, new string[] { args[0] } );
			}
			catch ( Exception ex )
			{
                Console.Error.WriteLine("Error: " + ex.Message);
				result = -2;
			}
			return result;
		}

	}
}
