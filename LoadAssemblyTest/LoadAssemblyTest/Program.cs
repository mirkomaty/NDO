using McMaster.NETCore.Plugins;
using NDOEnhancer;
using System.Reflection;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LoadAssemblyTest
{
	internal class Program
	{
		string pathToLoad;
		string projPath;
		List<Assembly> assemblies = new List<Assembly>();

		public Program()
		{
			this.pathToLoad = @"C:\Projekte\NDO\LoadAssemblyTest\NdoClasses\bin\Debug\netstandard2.0\NdoClasses.dll";
			this.projPath = Path.GetFullPath( Path.Combine( this.pathToLoad, @"..\..\..\.." ) );
		}

		static void Main( string[] args )
		{
			new Program().DoIt( args );
		}

		void ShowAssembly(Assembly assembly)
		{
			if (assemblies.Contains( assembly ))
				return;

			var types = assembly.GetTypes();
			foreach (var type in types)
			{
				Console.WriteLine( type.FullName );
				Console.WriteLine("-------------");

				if (type.Name == "PersistentClass")
					ShowAssembly( type.BaseType.Assembly );
				//if (type.BaseType != typeof( object ))
				//	Console.WriteLine( $"  Base: {type.BaseType.FullName}" );
			}

			assemblies.Add( assembly );
		}


		void DoIt( string[] args )
		{
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

			var loader = PluginLoader.CreateFromAssemblyFile(
			this.pathToLoad,
			sharedTypes: new Type[] {});
			using (loader.EnterContextualReflection())
			{
				var assembly = Assembly.Load("NdoClasses");
				ShowAssembly( assembly );
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
