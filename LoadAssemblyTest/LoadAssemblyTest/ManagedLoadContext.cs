using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace NDOEnhancer
{
	internal class ManagedLoadContext : AssemblyLoadContext
	{
		private readonly string _basePath;
		
		public ManagedLoadContext( string mainAssemblyPath )
		{
			_basePath = Path.GetDirectoryName( mainAssemblyPath ) ?? throw new ArgumentNullException( nameof( mainAssemblyPath ) );
		}

		/// <summary>
		/// Load an assembly.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		protected override Assembly? Load( AssemblyName assemblyName )
		{
			if (assemblyName.Name == null)
			{
				throw new ArgumentNullException( "assemblyName" );
			}
			
			var dllName = assemblyName.Name + ".dll";
			var localFile = Path.Combine(_basePath, dllName);
			if (File.Exists( localFile ))
			{
				return LoadFromAssemblyPath( localFile );
			}

			return null;
		}
	}
}
