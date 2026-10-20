using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StressTestClasses;

namespace StressTest
{
	/// <summary>
	/// Stress test for NDO: many workers create, change, query and delete objects concurrently
	/// using the async API of PersistenceManager and VirtualTable.
	/// </summary>
	/// <remarks>
	/// Exit codes: 0 = no errors, 1 = errors, 2 = cancelled, 3 = setup failed.
	/// </remarks>
	class Program
	{
		static readonly Dictionary<string, string> switchMappings = new Dictionary<string, string>
		{
			["--provider"] = "StressTest:Provider",
			["-p"] = "StressTest:Provider",
			["--connection"] = "StressTest:ConnectionString",
			["-c"] = "StressTest:ConnectionString",
			["--threads"] = "StressTest:Threads",
			["-t"] = "StressTest:Threads",
			["--iterations"] = "StressTest:Iterations",
			["-i"] = "StressTest:Iterations",
			["--duration"] = "StressTest:DurationSeconds",
			["-d"] = "StressTest:DurationSeconds",
			["--lines"] = "StressTest:LinesPerOrder",
			["--tags"] = "StressTest:TagCount",
			["--transactionMode"] = "StressTest:TransactionMode",
			["--isolationLevel"] = "StressTest:IsolationLevel",
			["--sharedMapping"] = "StressTest:SharedMapping",
			["--recreateSchema"] = "StressTest:RecreateSchema",
			["--dropSchema"] = "StressTest:DropSchemaAfterTest",
			["--stopOnFirstError"] = "StressTest:StopOnFirstError",
			["--maxErrors"] = "StressTest:MaxErrorsToPrint",
		};

		static async Task<int> Main( string[] args )
		{
			if (args.Contains( "--help" ) || args.Contains( "-h" ) || args.Contains( "-?" ))
			{
				PrintHelp();
				return 0;
			}

			StressTestOptions options;
			try
			{
				options = ReadOptions( args );
				options.Validate();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine( "Configuration error: " + ex.Message );
				Console.Error.WriteLine( "Use --help to see the options." );
				return 3;
			}

			Console.WriteLine( "NDO stress test" );
			Console.WriteLine( options );
			Console.WriteLine();

			using var cts = new CancellationTokenSource();
			Console.CancelKeyPress += ( s, e ) =>
			{
				e.Cancel = true;
				Console.WriteLine( "Cancelling..." );
				cts.Cancel();
			};

			// Avoid the slow thread injection of the thread pool, which would limit the concurrency at the beginning
			ThreadPool.GetMinThreads( out int minWorker, out int minIo );
			ThreadPool.SetMinThreads( Math.Max( minWorker, options.Threads * 2 ), Math.Max( minIo, options.Threads * 2 ) );

			TestDatabase db;
			int[] tagIds;
			TestDatabase.RowCounts before;
			try
			{
				db = new TestDatabase( options );
				if (options.RecreateSchema)
				{
					Console.WriteLine( "Creating the schema..." );
					await db.DropSchemaAsync( cts.Token );
					await db.CreateSchemaAsync( cts.Token );
				}
				tagIds = await GetOrCreateTagsAsync( db, options.TagCount, cts.Token );
				before = await db.GetRowCountsAsync( cts.Token );
				Console.WriteLine( "Rows before the test: " + before );
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine( "Setup failed:" );
				Console.Error.WriteLine( ex );
				return 3;
			}

			var statistics = new Statistics();
			string runId = Guid.NewGuid().ToString( "N" ).Substring( 0, 8 );
			DateTime? endTime = options.DurationSeconds > 0 ? DateTime.UtcNow.AddSeconds( options.DurationSeconds ) : (DateTime?) null;

			Console.WriteLine( $"Starting {options.Threads} workers (run id {runId})..." );
			var stopwatch = Stopwatch.StartNew();
			var tasks = Enumerable.Range( 0, options.Threads ).Select( workerNo => Task.Run( async () =>
			{
				try
				{
					await new Worker( workerNo, db, options, statistics, tagIds, runId ).RunAsync( endTime, cts.Token );
				}
				catch
				{
					// StopOnFirstError: stop the other workers
					cts.Cancel();
					throw;
				}
			} ) ).ToArray();

			Task all = Task.WhenAll( tasks );
			while (!all.IsCompleted)
			{
				await Task.WhenAny( all, Task.Delay( TimeSpan.FromSeconds( Math.Max( 1, options.ProgressIntervalSeconds ) ) ) );
				Console.WriteLine( $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] iterations ok: {statistics.SuccessfulIterations}, failed: {statistics.FailedIterations}" );
			}
			stopwatch.Stop();

			try
			{
				await all;
			}
			catch
			{
				// The errors are reported by the statistics
			}

			bool cancelled = cts.IsCancellationRequested && !options.StopOnFirstError;

			Console.WriteLine();
			Console.WriteLine( $"Elapsed: {stopwatch.Elapsed}" );
			Console.WriteLine( $"Iterations ok: {statistics.SuccessfulIterations}, failed: {statistics.FailedIterations}" );
			Console.WriteLine( $"Iterations/s: {statistics.SuccessfulIterations / stopwatch.Elapsed.TotalSeconds:F1}" );
			Console.Write( statistics.GetDeadlockReport() );
			Console.WriteLine();
			Console.WriteLine( statistics.GetOperationReport( stopwatch.Elapsed ) );

			bool consistent = true;
			try
			{
				var after = await db.GetRowCountsAsync();
				Console.WriteLine( "Rows after the test:  " + after );
				// Each successful iteration deletes all objects it has created.
				// If iterations failed, there might be remaining objects, so the check makes no sense.
				if (statistics.FailedIterations == 0 && !cancelled)
				{
					consistent = after.Customers == before.Customers
						&& after.Orders == before.Orders
						&& after.OrderLines == before.OrderLines
						&& after.Tags == before.Tags
						&& after.MappingTableEntries == before.MappingTableEntries;
					if (!consistent)
						Console.WriteLine( "ERROR: The row counts after the test differ from the row counts before the test." );
				}
			}
			catch (Exception ex)
			{
				consistent = false;
				Console.WriteLine( "ERROR: Can't read the row counts after the test:" );
				Console.WriteLine( ex );
			}

			if (statistics.ErrorCount > 0)
			{
				Console.WriteLine();
				Console.WriteLine( statistics.GetErrorReport( options.MaxErrorsToPrint ) );
			}

			bool success = statistics.ErrorCount == 0 && consistent && !cancelled;
			if (success && options.DropSchemaAfterTest)
			{
				Console.WriteLine( "Dropping the schema..." );
				await db.DropSchemaAsync();
			}

			Console.WriteLine();
			if (cancelled)
			{
				Console.WriteLine( "CANCELLED" );
				return 2;
			}
			Console.WriteLine( success ? "SUCCESS" : "FAILED" );
			return success ? 0 : 1;
		}

		static StressTestOptions ReadOptions( string[] args )
		{
			IConfiguration config = new ConfigurationBuilder()
				.SetBasePath( AppContext.BaseDirectory )
				.AddJsonFile( "appsettings.json", optional: false )
				// For local connection strings, which shouldn't be committed
				.AddJsonFile( "appsettings.local.json", optional: true )
				.AddEnvironmentVariables( "NDOSTRESS_" )
				.AddCommandLine( args, switchMappings )
				.Build();

			var options = config.GetSection( "StressTest" ).Get<StressTestOptions>() ?? new StressTestOptions();
			if (String.IsNullOrEmpty( options.ConnectionString ))
				options.ConnectionString = config.GetConnectionString( options.Provider );
			// Sqlite allows only one writer. A deferred transaction (ReadCommitted) which starts with a read can't be
			// upgraded to a write transaction, while another connection writes. Sqlite fails immediately with
			// "database is locked" instead of waiting. Serializable starts the transaction with BEGIN IMMEDIATE.
			if (options.IsolationLevel == null && String.Equals( options.Provider, "Sqlite", StringComparison.OrdinalIgnoreCase ))
				options.IsolationLevel = System.Data.IsolationLevel.Serializable;
			return options;
		}

		static async Task<int[]> GetOrCreateTagsAsync( TestDatabase db, int tagCount, CancellationToken ct )
		{
			await using (var pm = db.NewPersistenceManager())
			{
				var existing = await pm.Objects<Tag>().ToListAsync( ct );
				if (existing.Count >= tagCount)
					return existing.Select( t => (int) ((NDO.IPersistenceCapable) t).NDOObjectId.Id[0] ).ToArray();
			}
			await db.CreateTagsAsync( tagCount, ct );
			return await GetOrCreateTagsAsync( db, tagCount, ct );
		}

		static void PrintHelp()
		{
			Console.WriteLine( @"NDO stress test

Usage: dotnet run -- [options]

The defaults are read from appsettings.json and appsettings.local.json (optional).
Environment variables with the prefix NDOSTRESS_ override them, e.g. NDOSTRESS_ConnectionStrings__Postgre.

Options:
  -p, --provider <name>         Sqlite, SqlServer, MySql, MySqlConnector, Postgre
  -c, --connection <string>     Connection string (default: ConnectionStrings:<provider>)
  -t, --threads <n>             Number of concurrent workers
  -i, --iterations <n>          Iterations per worker
  -d, --duration <seconds>      Run for the given time instead of a fixed number of iterations
      --lines <n>               Lines per order (>= 2)
      --tags <n>                Number of shared tags (>= 3)
      --transactionMode <mode>  Optimistic or Pessimistic
      --isolationLevel <level>  ReadCommitted, Serializable, ... (default: NDO default, Sqlite: Serializable)
      --sharedMapping <bool>    All PersistenceManagers share one NDOMapping object
      --recreateSchema <bool>   Drop and create the tables before the test
      --dropSchema <bool>       Drop the tables after a successful test
      --stopOnFirstError <bool> Stop all workers after the first error
      --deadlockRetries <n>     Retries of a step after a deadlock (0: no retries)
      --maxErrors <n>           Number of errors printed with stack trace

Exit codes: 0 = success, 1 = errors, 2 = cancelled, 3 = configuration or setup error" );
		}
	}
}
