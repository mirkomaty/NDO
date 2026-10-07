//
// Copyright (c) 2002-2024 Mirko Matytschak
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

using System;
using Microsoft.Extensions.Logging;

namespace NDO.Application
{
	/// <summary>
	/// Simple logger, which writes the messages to the console using Console.WriteLine.
	/// </summary>
	/// <remarks>
	/// This logger is only used, if the application doesn't set up a host and doesn't call AddNdo and UseNdo.
	/// If a host is used, the logging configuration of the host applies and setting LogLevel has no effect.
	/// <code>
	/// NDOConsoleLogger.LogLevel = LogLevel.Warning;
	/// var pm = new PersistenceManager();
	/// </code>
	/// </remarks>
	public class NDOConsoleLogger : ILogger
	{
		private readonly string categoryName;

		/// <summary>
		/// Gets or sets the minimal log level. Messages with a lower log level are not written to the console.
		/// The default is LogLevel.Information.
		/// </summary>
		public static LogLevel LogLevel { get; set; } = LogLevel.Information;

		/// <summary>
		/// Constructs an NDOConsoleLogger object
		/// </summary>
		/// <param name="categoryName">The category name, which is written in front of each message.</param>
		public NDOConsoleLogger( string categoryName )
		{
			this.categoryName = categoryName;
		}

		/// <inheritdoc/>
		public IDisposable BeginScope<TState>( TState state )
		{
			return NullScope.Instance;
		}

		/// <inheritdoc/>
		public bool IsEnabled( LogLevel logLevel )
		{
			return logLevel != LogLevel.None && logLevel >= LogLevel;
		}

		/// <inheritdoc/>
		public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter )
		{
			if (!IsEnabled( logLevel ))
				return;

			string message = formatter != null ? formatter( state, exception ) : state?.ToString();
			if (string.IsNullOrEmpty( message ) && exception == null)
				return;

			Console.WriteLine( $"{logLevel}: {categoryName}: {message}" );
			if (exception != null)
				Console.WriteLine( exception.ToString() );
		}

		private class NullScope : IDisposable
		{
			public static readonly NullScope Instance = new NullScope();
			public void Dispose()
			{
			}
		}
	}

	/// <summary>
	/// Generic version of the NDOConsoleLogger, which uses the type name as category name.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NDOConsoleLogger<T> : NDOConsoleLogger, ILogger<T>
	{
		/// <summary>
		/// Constructs an NDOConsoleLogger&lt;T&gt; object
		/// </summary>
		public NDOConsoleLogger() : base( typeof( T ).FullName )
		{
		}
	}

	/// <summary>
	/// ILoggerFactory implementation, which creates NDOConsoleLogger objects.
	/// </summary>
	internal class NDOConsoleLoggerFactory : ILoggerFactory
	{
		public static readonly NDOConsoleLoggerFactory Instance = new NDOConsoleLoggerFactory();

		public ILogger CreateLogger( string categoryName )
		{
			return new NDOConsoleLogger( categoryName );
		}

		public void AddProvider( ILoggerProvider provider )
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}
	}
}
