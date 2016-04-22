using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDO.Logging;

namespace NdoUnitTests
{
	class TestLogAdapter : ILogAdapter
	{
		StringBuilder text = new StringBuilder();

		public void Clear()
		{
			text = new StringBuilder();
		}

		public void Debug( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public void Error( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public void Info( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public void Warn( string message )
		{
			this.text.Append( message );
			this.text.Append( "\r\n" );
		}

		public string Text
		{
			get
			{
				return this.text.ToString();
			}
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
