using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.ChangeLogging
{
	/// <summary>
	/// Interface for a class which is used to convert objects to strings.
	/// </summary>
	/// <remarks>
	/// For example, the conversion can be done by JsonConvert.SerializeObject.
	/// </remarks>
	public interface IChangeLogConverter
	{
		/// <summary>
		/// Converts an object to a string that represents a name-value pair.
		/// </summary>
		/// <param name="o">The object to serialize.</param>
		/// <returns></returns>
		string ToString( object o );
	}
}
