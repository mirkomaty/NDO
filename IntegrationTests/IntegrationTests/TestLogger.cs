//
// Copyright (c) 2002-2023 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace NdoUnitTests
{
	class TestLogger : ILogger, IDisposable
	{
		StringBuilder sb = new StringBuilder();
		private readonly string cat;
		public override string ToString()
		{
			return base.ToString();
		}

		//public static event Action<string> EmitLog;

		public TestLogger( string cat )
		{
			this.cat = cat;
		}
		public IDisposable BeginScope<TState>( TState state )
		{
			return this;
		}

		public void Dispose()
		{
		}

		public bool IsEnabled( LogLevel logLevel )
		{
			return true;
		}

		public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter )
		{
			if (state == null)
				return;

			sb.Append( state.ToString() );
			sb.Append( "\r\n" );
			//var s = $"{logLevel}: {formatter( state, exception )}";
			//Debug.WriteLine( $"{cat}: {s}" );
			//if (EmitLog != null)
			//	EmitLog( s );
		}

		public void Clear()
		{
			this.sb = new StringBuilder();
		}

		public string Text => this.sb.ToString();
	}

	class TestLogger<T> : TestLogger, ILogger<T>
	{
		public TestLogger() : base( typeof( T ).ToString() )
		{
		}
	}
}
