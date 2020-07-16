using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// Constructs an object of a given type. Returns the same object for the same constructor.
	/// </summary>
	public class TypeMappingResolver : IResolver
	{
		private readonly Type tFrom;
		private readonly Type tTo;
		private readonly INDOContainer configContainer;
		ConcurrentDictionary<string,object> values = new ConcurrentDictionary<string, object>();
		string name;

		/// <summary>
		/// Constructs a TypeMappingResolver object
		/// </summary>
		public TypeMappingResolver(INDOContainer configContainer, Type tFrom, Type tTo, string name)
		{
			this.tFrom = tFrom;
			this.tTo = tTo;
			this.configContainer = configContainer;
			this.name = name ?? string.Empty;
		}

		/// <summary>
		/// Resolves an object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="overrides"></param>
		/// <returns></returns>
		public object Resolve( string name, ParameterOverride[] overrides )
		{
			var key = name ?? this.name;

			var result = this.values.GetOrAdd( key, _ =>
			{
				List<object> parameters = new List<object>();

				var registrations = this.configContainer.Registrations;
				ConstructorInfo constructor = null;
				foreach (var constr in tTo.GetConstructors().OrderBy( ci => ci.GetParameters().Length ))
				{
#warning Wir müssen hier die Konstruktoren nach overrides filtern.
					bool canResolve = true;
					parameters.Clear();

					foreach (var p in constr.GetParameters())
					{
						if (!overrides.Any(ov=>ov.Name == p.Name) && !registrations.Any( r => r.Key == p.ParameterType ))
						{
							canResolve = false;
							break;
						}
					}

					if (canResolve)
					{
						constructor = constr;
						//isDefaultConstructor = constr.GetParameters().Length == 0;
						break;
					}
				}


				if (constructor == null)
					throw new NDOException( 116, $"Cant find a resolvable constructor for class '{tFrom.FullName}'" );

				foreach (var p in constructor.GetParameters())
				{
					var overrde = overrides.FirstOrDefault(ov=>ov.Name == p.Name);
					if (overrde != null)
						parameters.Add( overrde.Value );
					else
						parameters.Add( this.configContainer.Resolve( p.ParameterType ) );
				}

				return constructor.Invoke( parameters.ToArray() );
			} );

			return result;
		}
	}
}
