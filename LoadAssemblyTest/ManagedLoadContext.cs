// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using McMaster.NETCore.Plugins.LibraryModel;

namespace McMaster.NETCore.Plugins.Loader
{
	/// <summary>
	/// An implementation of <see cref="AssemblyLoadContext" /> which attempts to load managed and native
	/// binaries at runtime immitating some of the behaviors of corehost.
	/// </summary>
	[DebuggerDisplay( "'{Name}' ({_mainAssemblyPath})" )]
	internal class ManagedLoadContext : AssemblyLoadContext
	{
		private readonly string _basePath;
		private readonly string _mainAssemblyPath;
		private readonly IReadOnlyDictionary<string, ManagedLibrary> _managedAssemblies;
		private readonly IReadOnlyCollection<string> _privateAssemblies;
		private readonly ICollection<string> _defaultAssemblies;
		private readonly IReadOnlyCollection<string> _additionalProbingPaths;
		private readonly bool _preferDefaultLoadContext;
		private readonly string[] _resourceRoots;
		private readonly bool _loadInMemory;
		private readonly bool _lazyLoadReferences;
		private readonly AssemblyLoadContext _defaultLoadContext;

		public ManagedLoadContext( string mainAssemblyPath,
			IReadOnlyDictionary<string, ManagedLibrary> managedAssemblies,
			IReadOnlyCollection<string> privateAssemblies,
			IReadOnlyCollection<string> defaultAssemblies,
			IReadOnlyCollection<string> additionalProbingPaths,
			IReadOnlyCollection<string> resourceProbingPaths,
			AssemblyLoadContext defaultLoadContext,
			bool preferDefaultLoadContext,
			bool lazyLoadReferences,
			bool loadInMemory )
		{
			if (resourceProbingPaths == null)
			{
				throw new ArgumentNullException( nameof( resourceProbingPaths ) );
			}

			_mainAssemblyPath = mainAssemblyPath ?? throw new ArgumentNullException( nameof( mainAssemblyPath ) );

			_basePath = Path.GetDirectoryName( mainAssemblyPath ) ?? throw new ArgumentException( nameof( mainAssemblyPath ) );
			_managedAssemblies = managedAssemblies ?? throw new ArgumentNullException( nameof( managedAssemblies ) );
			_privateAssemblies = privateAssemblies ?? throw new ArgumentNullException( nameof( privateAssemblies ) );
			_defaultAssemblies = defaultAssemblies != null ? defaultAssemblies.ToList() : throw new ArgumentNullException( nameof( defaultAssemblies ) );
			_additionalProbingPaths = additionalProbingPaths ?? throw new ArgumentNullException( nameof( additionalProbingPaths ) );
			_defaultLoadContext = defaultLoadContext;
			_preferDefaultLoadContext = preferDefaultLoadContext;
			_loadInMemory = loadInMemory;
			_lazyLoadReferences = lazyLoadReferences;

			_resourceRoots = new[] { _basePath }
				.Concat( resourceProbingPaths )
				.ToArray();
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
				// not sure how to handle this case. It's technically possible.
				return null;
			}

			if (( _preferDefaultLoadContext || _defaultAssemblies.Contains( assemblyName.Name ) ) && !_privateAssemblies.Contains( assemblyName.Name ))
			{
				// If default context is preferred, check first for types in the default context unless the dependency has been declared as private
				try
				{
					var defaultAssembly = _defaultLoadContext.LoadFromAssemblyName(assemblyName);
					if (defaultAssembly != null)
					{
						// Add referenced assemblies to the list of default assemblies.
						// This is basically lazy loading
						if (_lazyLoadReferences)
						{
							foreach (var reference in defaultAssembly.GetReferencedAssemblies())
							{
								if (reference.Name != null && !_defaultAssemblies.Contains( reference.Name ))
								{
									_defaultAssemblies.Add( reference.Name );
								}
							}
						}

						// Older versions used to return null here such that returned assembly would be resolved from the default ALC.
						// However, with the addition of custom default ALCs, the Default ALC may not be the user's chosen ALC when
						// this context was built. As such, we simply return the Assembly from the user's chosen default load context.
						return defaultAssembly;
					}
				}
				catch
				{
					// Swallow errors in loading from the default context
				}
			}



			// Resource assembly binding does not use the TPA. Instead, it probes PLATFORM_RESOURCE_ROOTS (a list of folders)
			// for $folder/$culture/$assemblyName.dll
			// See https://github.com/dotnet/coreclr/blob/3fca50a36e62a7433d7601d805d38de6baee7951/src/binder/assemblybinder.cpp#L1232-L1290

			if (!string.IsNullOrEmpty( assemblyName.CultureName ) && !string.Equals( "neutral", assemblyName.CultureName ))
			{
				foreach (var resourceRoot in _resourceRoots)
				{
					var resourcePath = Path.Combine(resourceRoot, assemblyName.CultureName, assemblyName.Name + ".dll");
					if (File.Exists( resourcePath ))
					{
						return LoadAssemblyFromFilePath( resourcePath );
					}
				}

				return null;
			}

			if (_managedAssemblies.TryGetValue( assemblyName.Name, out var library ) && library != null)
			{
				if (SearchForLibrary( library, out var path ) && path != null)
				{
					return LoadAssemblyFromFilePath( path );
				}
			}
			else
			{
				// if an assembly was not listed in the list of known assemblies,
				// fallback to the load context base directory
				var dllName = assemblyName.Name + ".dll";
				foreach (var probingPath in _additionalProbingPaths.Prepend( _basePath ))
				{
					var localFile = Path.Combine(probingPath, dllName);
					if (File.Exists( localFile ))
					{
						return LoadAssemblyFromFilePath( localFile );
					}
				}
			}

			return null;
		}

		public Assembly LoadAssemblyFromFilePath( string path )
		{
			if (!_loadInMemory)
			{
				return LoadFromAssemblyPath( path );
			}

			using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			var pdbPath = Path.ChangeExtension(path, ".pdb");
			if (File.Exists( pdbPath ))
			{
				using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				return LoadFromStream( file, pdbFile );
			}
			return LoadFromStream( file );

		}

		private bool SearchForLibrary( ManagedLibrary library, out string? path )
		{
			// 1. Check for in _basePath + app local path
			var localFile = Path.Combine(_basePath, library.AppLocalPath);
			if (File.Exists( localFile ))
			{
				path = localFile;
				return true;
			}

			// 2. Search additional probing paths
			foreach (var searchPath in _additionalProbingPaths)
			{
				var candidate = Path.Combine(searchPath, library.AdditionalProbingPath);
				if (File.Exists( candidate ))
				{
					path = candidate;
					return true;
				}
			}

			// 3. Search in base path
			foreach (var ext in ManagedAssemblyExtensions)
			{
				var local = Path.Combine(_basePath, library.Name.Name + ext);
				if (File.Exists( local ))
				{
					path = local;
					return true;
				}
			}

			path = null;
			return false;
		}

		public static readonly string[] ManagedAssemblyExtensions = new[]
		{
				".dll",
				".ni.dll",
				".exe",
				".ni.exe"
		};
	}
}
