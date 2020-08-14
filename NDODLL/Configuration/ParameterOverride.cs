using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// Class which helps overriding constructor parameters
	/// </summary>
	public class ParameterOverride
	{
		/// <summary />
		public string Name { get; }
		/// <summary />
		public object Value { get; }

		/// <summary>
		/// Creates a ParameterOverride object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public ParameterOverride(string name, object value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Creates a ParameterOverride object
		/// </summary>
		/// <param name="value"></param>
		public ParameterOverride(object value)
		{
			Name = null;
			Value = value;
		}
	}
}
