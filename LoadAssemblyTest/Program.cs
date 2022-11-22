using McMaster.NETCore.Plugins;
using NDOEnhancer;
using System.Reflection;
using System.IO;
using System;
using System.Linq;

namespace LoadAssemblyTest
{
	internal class Program
	{
		string pathToLoad;
		string projPath;

		public Program()
		{
			this.pathToLoad = @"C:\Projekte\Laykit 3\BusinessClasses\bin\Debug\netstandard2.0\BusinessClasses.dll";
			this.projPath = Path.GetFullPath( Path.Combine( this.pathToLoad, @"..\..\..\.." ) );
		}

		static void Main( string[] args )
		{
			new Program().DoIt( args );
		}

		void DoIt( string[] args )
		{
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

			var loader = PluginLoader.CreateFromAssemblyFile(
			this.pathToLoad,
			sharedTypes: new Type[] {});
			using (loader.EnterContextualReflection())
			{
				var assembly = Assembly.Load("BusinessClasses");
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					Console.WriteLine( type.FullName );
				}
			}
		}

		private Assembly? OnAssemblyResolve( object? sender, ResolveEventArgs args )
		{
			Console.WriteLine( $"AssemblyResolve: {args?.Name}" );
			if (args == null)
				throw new ArgumentNullException( "args" );

			var assName = args.Name.Split(',').FirstOrDefault();
			if (assName == null)
				return null;

			string? path = null;
				path = GetPackageLibPath( assName );

			if (path == null)
				return null;

			Console.WriteLine( "Location: " + path );
			if (File.Exists( path ))
			{
				return Assembly.LoadFrom( path );
			}
			return null;
		}

		string? GetPackageLibPath( string assName )
		{
			var packageName = assName.ToLowerInvariant();
			if (assName.Equals( "NDO", StringComparison.OrdinalIgnoreCase ))
				packageName = "ndo.dll";

			string? nugetBasePath = new NugetProps(this.pathToLoad).DefaultNugetPackageFolder;
			if (nugetBasePath == null)
				throw new Exception("Nuget-Folder not found");

			var packageDir = new ProjectAssets(this.projPath).GetPackageDir( packageName );
			if (packageDir == null)
				return null;
			return Path.Combine( nugetBasePath, packageDir );
		}
	}
}
