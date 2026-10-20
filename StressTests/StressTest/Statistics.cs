using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StressTest
{
	/// <summary>
	/// Collects the durations of the database operations and the errors of all workers. Thread safe.
	/// </summary>
	public class Statistics
	{
		class OperationStats
		{
			public long Count;
			public long TotalTicks;
			public long MaxTicks;

			public void Add( long ticks )
			{
				Interlocked.Increment( ref Count );
				Interlocked.Add( ref TotalTicks, ticks );
				long max;
				while (ticks > (max = Interlocked.Read( ref MaxTicks )))
				{
					if (Interlocked.CompareExchange( ref MaxTicks, ticks, max ) == max)
						break;
				}
			}
		}

		public class ErrorInfo
		{
			public int Worker { get; init; }
			public int Iteration { get; init; }
			public string Step { get; init; }
			public Exception Exception { get; init; }
		}

		readonly ConcurrentDictionary<string, OperationStats> operations = new ConcurrentDictionary<string, OperationStats>();
		readonly ConcurrentQueue<ErrorInfo> errors = new ConcurrentQueue<ErrorInfo>();
		readonly ConcurrentDictionary<string, long> deadlockRetries = new ConcurrentDictionary<string, long>();
		long successfulIterations;
		long failedIterations;

		public long SuccessfulIterations => Interlocked.Read( ref this.successfulIterations );
		public long FailedIterations => Interlocked.Read( ref this.failedIterations );
		public int ErrorCount => this.errors.Count;

		public void IterationSucceeded() => Interlocked.Increment( ref this.successfulIterations );

		public void IterationFailed( ErrorInfo error )
		{
			Interlocked.Increment( ref this.failedIterations );
			this.errors.Enqueue( error );
		}

		/// <summary>
		/// Counts a retry of a step after a deadlock.
		/// </summary>
		public void DeadlockRetried( string step ) => this.deadlockRetries.AddOrUpdate( step, 1, ( _, n ) => n + 1 );

		public string GetDeadlockReport()
		{
			var sb = new StringBuilder();
			sb.AppendLine( $"Deadlock retries: {this.deadlockRetries.Values.Sum()}" );
			foreach (var kvp in this.deadlockRetries.OrderBy( k => k.Key ))
				sb.AppendLine( $"  {kvp.Value,6} x {kvp.Key}" );
			return sb.ToString();
		}

		/// <summary>
		/// Executes an operation and measures its duration.
		/// </summary>
		public async Task<T> MeasureAsync<T>( string operation, Func<Task<T>> func )
		{
			long start = Stopwatch.GetTimestamp();
			T result = await func();
			Add( operation, Stopwatch.GetTimestamp() - start );
			return result;
		}

		/// <summary>
		/// Executes an operation and measures its duration.
		/// </summary>
		public async Task MeasureAsync( string operation, Func<Task> func )
		{
			long start = Stopwatch.GetTimestamp();
			await func();
			Add( operation, Stopwatch.GetTimestamp() - start );
		}

		void Add( string operation, long ticks )
		{
			this.operations.GetOrAdd( operation, _ => new OperationStats() ).Add( ticks );
		}

		static double Ms( long ticks ) => ticks * 1000.0 / Stopwatch.Frequency;

		public string GetOperationReport( TimeSpan elapsed )
		{
			var sb = new StringBuilder();
			sb.AppendLine( $"{"Operation",-34} {"Count",9} {"Avg ms",9} {"Max ms",9} {"Ops/s",9}" );
			foreach (var kvp in this.operations.OrderBy( k => k.Key ))
			{
				var s = kvp.Value;
				double avg = s.Count == 0 ? 0 : Ms( s.TotalTicks ) / s.Count;
				double perSecond = elapsed.TotalSeconds == 0 ? 0 : s.Count / elapsed.TotalSeconds;
				sb.AppendLine( $"{kvp.Key,-34} {s.Count,9} {avg,9:F1} {Ms( s.MaxTicks ),9:F1} {perSecond,9:F1}" );
			}
			return sb.ToString();
		}

		public string GetErrorReport( int maxErrors )
		{
			var sb = new StringBuilder();
			var all = this.errors.ToArray();
			sb.AppendLine( "Errors grouped by step and exception type:" );
			foreach (var g in all.GroupBy( e => (e.Step, Type: e.Exception.GetType().FullName) ).OrderByDescending( g => g.Count() ))
				sb.AppendLine( $"  {g.Count(),6} x {g.Key.Step}: {g.Key.Type}" );

			sb.AppendLine();
			sb.AppendLine( $"The first {Math.Min( maxErrors, all.Length )} errors:" );
			foreach (var e in all.Take( maxErrors ))
			{
				sb.AppendLine( $"--- Worker {e.Worker}, iteration {e.Iteration}, step '{e.Step}':" );
				sb.AppendLine( e.Exception.ToString() );
			}
			return sb.ToString();
		}
	}
}
