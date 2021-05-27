using System;

namespace NDO.Linq
{
	/// <summary>
	/// Attribute to rename a server function
	/// </summary>
	/// <remarks>
	/// If a method has this attribute, the attribute value is used as the name of the server function instead of the method name.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method)]
	public class ServerFunctionAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the name to be used for the server function
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Constructs a ServerFunctionAttribute object
		/// </summary>
		/// <param name="serverFunctionName"></param>
		public ServerFunctionAttribute(string serverFunctionName)
		{
			Name = serverFunctionName;
		}
	}
}
