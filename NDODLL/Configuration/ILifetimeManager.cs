using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public interface ILifetimeManager
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <param name="tFrom"></param>
		/// <param name="tTo"></param>
		IResolver CreateResolver(Type tFrom, Type tTo);
	}
}
