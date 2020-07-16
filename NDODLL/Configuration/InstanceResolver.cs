using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// Resolves registered instances
	/// </summary>
	public class InstanceResolver : IResolver
	{
		ConcurrentDictionary<string, object> values = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// Constructs an InstanceResolver
		/// </summary>
		public InstanceResolver()
		{
		}

		/// <summary>
		/// Constructs an InstanceResolver
		/// </summary>
		/// <param name="name"></param>
		/// <param name="o"></param>
		public void AddOrUpdate(string name, object o)
		{
			if (name == null)
				name = String.Empty;

			Func<string, object> valueFactory = _ => o;

			this.values.AddOrUpdate( name, o, ( k, old ) => valueFactory( k ) );
		}

		///<inheritdoc/>
		public object Resolve( string name, ParameterOverride[] overrides )
		{
			if (values.TryGetValue( name ?? string.Empty, out object result ))
				return result;

			return null;
		}
	}
}
