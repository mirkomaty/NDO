using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NDO.Configuration
{
	/// <summary>
	/// Constructs an object of a given type. Returns the same object for the same constructor.
	/// </summary>
	class TypeMappingResolver : IResolver
	{
		private readonly Type tFrom;
		private readonly Type tTo;

		/// <summary>
		/// Constructs a TypeMappingResolver object
		/// </summary>
		public TypeMappingResolver( Type tFrom, Type tTo )
		{
			this.tFrom = tFrom;
			this.tTo = tTo;
		}

		/// <inheritdoc/>
		public virtual object Resolve( INDOContainer resolvingContainer, string name, ParameterOverride[] overrides )
		{
			return CreateObject( resolvingContainer, overrides );
		}

		/// <summary>
		/// Creates an object and resolves the parameters
		/// </summary>
		/// <param name="resolvingContainer"></param>
		/// <param name="overrides"></param>
		/// <returns></returns>
		protected virtual object CreateObject( INDOContainer resolvingContainer, ParameterOverride[] overrides )
		{
			List<object> parameters = new List<object>();

			var registrations = resolvingContainer.Registrations;
			ConstructorInfo constructor = null;
			foreach (var constr in tTo.GetConstructors().OrderBy( ci => ci.GetParameters().Length ))
			{
				bool canResolve = true;
				parameters.Clear();

				foreach (var p in constr.GetParameters())
				{
					if (p.ParameterType != typeof( INDOContainer ) 
						&& !overrides.Any( ov => ov.Name == null && p.ParameterType.IsAssignableFrom(ov.Value.GetType()) || ov.Name == p.Name ) 
						&& !registrations.Any( r => r.Key == p.ParameterType ))
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
				if (p.ParameterType == typeof( INDOContainer ))
				{
					parameters.Add( resolvingContainer );
					continue;
				}
				var overrde = overrides.FirstOrDefault(ov=> ov.Name == null && p.ParameterType.IsAssignableFrom(ov.Value.GetType()) || ov.Name == p.Name);
				if (overrde != null)
					parameters.Add( overrde.Value );
				else
					parameters.Add( resolvingContainer.Resolve( p.ParameterType ) );
			}

			return constructor.Invoke( parameters.ToArray() );
		}

		///<inheritdoc/>
		public virtual void Dispose()
		{
		}

	}
}
