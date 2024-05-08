namespace NDO
{
	/// <summary>
	/// DI-Interface to get a scoped version of the PM.
	/// </summary>
	public interface IPersistenceManagerAccessor
	{
		/// <summary>
		/// Gets the PersistenceManager of the scope.
		/// </summary>
		PersistenceManager PersistenceManager { get; set; }
	}
}
