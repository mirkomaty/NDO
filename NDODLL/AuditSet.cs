using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDO
{
	/// <summary>
	/// This collection of objects is provided by the OnSaved event
	/// </summary>
	public class AuditSet
	{
		public IEnumerable<IPersistenceCapable> CreatedObjects { get; set; }
		public IEnumerable<IPersistenceCapable> DeletedObjects { get; set; }
		public IEnumerable<IPersistenceCapable> ChangedObjects { get; set; }
	}
}
