using System;
using System.Data;
using System.Text;
using NDO;

namespace StressTest
{
	/// <summary>
	/// Options of the stress test. Bound to the section "StressTest" of the configuration.
	/// </summary>
	public class StressTestOptions
	{
		public string Provider { get; set; } = "Sqlite";
		public string ConnectionString { get; set; }
		public int Threads { get; set; } = 32;
		public int Iterations { get; set; } = 25;
		public int DurationSeconds { get; set; }
		public int LinesPerOrder { get; set; } = 5;
		public int TagCount { get; set; } = 8;
		public TransactionMode TransactionMode { get; set; } = TransactionMode.Optimistic;
		/// <summary>
		/// Isolation level of the transactions. Null means: use the default of NDO.
		/// </summary>
		public IsolationLevel? IsolationLevel { get; set; }
		public bool SharedMapping { get; set; } = true;
		public bool RecreateSchema { get; set; } = true;
		public bool DropSchemaAfterTest { get; set; }
		public bool StopOnFirstError { get; set; }
		/// <summary>
		/// Number of retries of a step, if the database chose the transaction as deadlock victim. 0 turns off the retries.
		/// </summary>
		public int DeadlockRetries { get; set; } = 5;
		public int MaxErrorsToPrint { get; set; } = 20;
		public int ProgressIntervalSeconds { get; set; } = 5;

		public void Validate()
		{
			if (String.IsNullOrEmpty( Provider ))
				throw new ArgumentException( "No provider given." );
			if (String.IsNullOrEmpty( ConnectionString ))
				throw new ArgumentException( $"No connection string for provider '{Provider}' given. Add it to the section ConnectionStrings in appsettings.json or use --connection." );
			if (Threads < 1)
				throw new ArgumentException( "Threads must be > 0." );
			if (Iterations < 1 && DurationSeconds < 1)
				throw new ArgumentException( "Either Iterations or DurationSeconds must be > 0." );
			// The worker removes one line and adds one line, so we need at least two lines
			if (LinesPerOrder < 2)
				throw new ArgumentException( "LinesPerOrder must be >= 2." );
			// Each order gets two tags, the update replaces one of them by a third tag
			if (TagCount < 3)
				throw new ArgumentException( "TagCount must be >= 3." );
			if (DeadlockRetries < 0)
				throw new ArgumentException( "DeadlockRetries must be >= 0." );
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine( $"Provider:          {Provider}" );
			sb.AppendLine( $"Connection string: {MaskPassword( ConnectionString )}" );
			sb.AppendLine( $"Threads:           {Threads}" );
			if (DurationSeconds > 0)
				sb.AppendLine( $"Duration:          {DurationSeconds} s" );
			else
				sb.AppendLine( $"Iterations:        {Iterations} per thread" );
			sb.AppendLine( $"Lines per order:   {LinesPerOrder}" );
			sb.AppendLine( $"Tags:              {TagCount}" );
			sb.AppendLine( $"TransactionMode:   {TransactionMode}" );
			sb.AppendLine( $"IsolationLevel:    {(IsolationLevel.HasValue ? IsolationLevel.ToString() : "NDO default")}" );
			sb.AppendLine( $"Shared mapping:    {SharedMapping}" );
			sb.Append(     $"Deadlock retries:  {DeadlockRetries}" );
			return sb.ToString();
		}

		static string MaskPassword( string connectionString )
		{
			if (connectionString == null)
				return null;
			var parts = connectionString.Split( ';' );
			for (int i = 0; i < parts.Length; i++)
			{
				int eq = parts[i].IndexOf( '=' );
				if (eq < 0)
					continue;
				string key = parts[i].Substring( 0, eq ).Trim();
				if (key.Equals( "Password", StringComparison.OrdinalIgnoreCase ) || key.Equals( "Pwd", StringComparison.OrdinalIgnoreCase ))
					parts[i] = key + "=***";
			}
			return String.Join( ";", parts );
		}
	}
}
