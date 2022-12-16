
namespace NDO.Logging
{
	/// <summary>
	/// Implementation of a LogAdapter using the LogAdapter of the
	/// PersistenceManager
	/// </summary>
	public class PmLogAdapter : ILogAdapter
	{
		private readonly PersistenceManagerBase pm;

		/// <summary>
		/// Constructs a PmLogAdapter object
		/// </summary>
		/// <param name="pm"></param>
		public PmLogAdapter( PersistenceManagerBase pm )
		{
			this.pm = pm;
		}

		///<inheritdoc />
		public void Clear()
		{			
		}

		/// <inheritdoc />
		public void Debug( string message )
		{
			if (this.pm.VerboseMode && this.pm.LogAdapter != null)
				this.pm.LogAdapter.Debug( message );
		}

		/// <inheritdoc/>
		public void Error( string message )
		{
			if (this.pm.VerboseMode && this.pm.LogAdapter != null)
				this.pm.LogAdapter.Error( message );
		}

		/// <inheritdoc/>
		public void Info( string message )
		{
			if (this.pm.VerboseMode && this.pm.LogAdapter != null)
				this.pm.LogAdapter.Info( message );
		}

		/// <inheritdoc/>
		public void Warn( string message )
		{
			if (this.pm.VerboseMode && this.pm.LogAdapter != null)
				this.pm.LogAdapter.Warn( message );
		}
	}
}
