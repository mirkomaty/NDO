using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO.Configuration
{
	class ContainerControlledTypeMappingResolver : TypeMappingResolver
	{
		ConcurrentDictionary<string,object> values = new ConcurrentDictionary<string, object>();

		public ContainerControlledTypeMappingResolver(Type tFrom, Type tTo) : base (tFrom, tTo)
		{
		}
		/// <inheritdoc/>
		public override object Resolve( INDOContainer resolvingContainer, string name, ParameterOverride[] overrides )
		{
			var key = name ?? string.Empty;

			var result = this.values.GetOrAdd( key, _ =>
			{
				return CreateObject( resolvingContainer, overrides );
			} );

			return result;
		}

		///<inheritdoc/>
		public override void Dispose()
		{
			foreach (IDisposable d in values.Values.Where( o => o is IDisposable ))
				d.Dispose();

			values.Clear();
		}
	}
}
