using System;
using System.Collections.Generic;
using System.Text;

namespace NDO.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public class TransientLifetime : ILifetime
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="createInstance"></param>
		/// <param name="scope"></param>
		/// <returns></returns>
		public object GetInstance( Func<object> createInstance, Scope scope )
		{
			return createInstance();
		}
	}
}
