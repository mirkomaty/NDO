using System;

namespace NDO.Query
{
	/// <summary>
	/// Specialized Exception class to be used in the context of Queries
	/// </summary>
	public class QueryException : NDOException
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="errorNr">The unique NDO error number.</param>
		/// <param name="msg">Exception message.</param>
		/// <remarks>
		/// QueryException is derived from NDOException. The error numbers don't overlap 
		/// with the error numbers used in NDOException.
		/// </remarks>
		public QueryException(int errorNr, string msg) : base (errorNr, "Query Exception: " + msg)
		{
		}
	}
}
