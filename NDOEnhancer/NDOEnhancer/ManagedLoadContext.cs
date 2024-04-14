using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace NDOEnhancer
{
	internal class ManagedLoadContext : AssemblyLoadContext
	{
		private readonly string _basePath;
		private readonly bool verboseMode;

		public ManagedLoadContext( string basePath, bool verboseMode ) : base( true )
		{
			_basePath = basePath ?? throw new ArgumentNullException( nameof( basePath ) );
			this.verboseMode = verboseMode;
		}

		/// <summary>
		/// Load an assembly.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		protected override Assembly? Load( AssemblyName assemblyName )
		{
			if (this.verboseMode)
				Console.WriteLine($"ManagedLoadContext: Loading: {assemblyName}");
            if (assemblyName.Name == null)
			{
				throw new ArgumentNullException( "assemblyName" );
			}

			if (assemblyName.Name == "NDO")
				return null; // Use DefaultContext

			var dllName = assemblyName.Name + ".dll";

			foreach (var dir in new[] { "org", String.Empty })
			{
				string localFile;
				if (dir != String.Empty)
					localFile = Path.Combine(_basePath, dir, dllName);
				else
					localFile = Path.Combine( _basePath, dllName );

				if (File.Exists( localFile ))
				{
					if (this.verboseMode)
						Console.WriteLine( $"ManagedLoadContext: Found: {localFile}" );
					return LoadFromAssemblyPath( localFile );
				}
			}

			return null;
		}
	}
}
