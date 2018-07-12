using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace NDO.Query
{
	public class FetchGroup<TModel> : List<string>
	{
		/// <summary>
		/// Initializes a FetchGroup object with the name of the fields to fetch.
		/// </summary>
		/// <param name="fields"></param>
		public FetchGroup (IEnumerable<string> fields)
		{
			foreach (string s in fields)
			{
				Add( s );
			}
		}
		
		/// <summary>
		/// Initializes a FetchGroup object with expressions representing the name of the fields to fetch.
		/// </summary>
		/// <param name="fields"></param>
		/// <remarks>Note: Names of Accessor Properties will be mapped to the underlying fields.</remarks>
		public FetchGroup(params Expression<Func<TModel, object>>[] fields )
		{
			int i = 0;
			foreach (Expression ex in fields)
			{
				MemberExpression mex = ((LambdaExpression)ex).Body as MemberExpression;
				if (mex == null)
					throw new ArgumentException( "Parameter should be a MemberExpression", "fields[" + i + "]" );
				Add( mex.Member.Name );
				i++;
			}
		}
	}
}
