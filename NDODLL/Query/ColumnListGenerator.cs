using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDOql.Expressions;
using NDO.Mapping;

namespace NDO.Query
{
	/// <summary>
	/// Helper class to generate Column names of the select statement
	/// </summary>
	internal class ColumnListGenerator
	{
		Class cls;
		bool hollowResults;

		internal ColumnListGenerator(Class cls, bool hollowResults)
		{
			this.cls = cls;
			this.hollowResults = hollowResults;
		}

		internal string GenerateColumnList(OqlExpression expressionTree)
		{
			//StringBuilder sb = new StringBuilder();
			//foreach (Field f in cls.Fields)
			//{
			//	sb.Append( QualifiedColumnName.Get( f.Column ) );
			//	sb.Append( ", " );
			//}
			//sb.Length -= 2;
			//return sb.ToString();
			return FieldMarker.Instance;
		}
	}
}
