using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NDOql.Expressions;

namespace NDOql
{
	public class OqlParser
	{
		public OqlExpression Parse( string oql )
		{
			if (String.IsNullOrEmpty( oql ))
				return null;
			MemoryStream stream = new MemoryStream();
			StreamWriter sw = new StreamWriter( stream, Encoding.UTF8 );
			sw.Write( oql );
			sw.Flush();
			stream.Position = 0;
			Parser parser = new Parser( new Scanner( stream ) );
			parser.Parse();
			OqlExpression result = parser.RootExpression;
			AddParents( result );
			return result;
		}

		private void AddParents( OqlExpression exp )
		{
			foreach ( OqlExpression child in exp.Children )
			{
				( (IManageExpression) child ).SetParent( exp );
				AddParents( child );
			}
		}
	}
}
