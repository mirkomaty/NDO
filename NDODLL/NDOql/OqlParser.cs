﻿using System;
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
			MemoryStream ms = new MemoryStream();
			StreamWriter errSw = new StreamWriter( ms );
			parser.errors.errorStream = errSw;
			parser.Parse();
			if (parser.errors.count > 0)
			{
				errSw.Flush();
				ms.Seek( 0L, SeekOrigin.Begin );
				string errString = string.Empty;
				using (StreamReader sr = new StreamReader( ms ))
					errString = sr.ReadToEnd();
				throw new OqlExpressionException( "Parser Errors: " + errString );
			}
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
