using System;
using System.Linq;
using System.Collections.Generic;
using NDO;

namespace PureBusinessClasses.DITests
{
	public interface IDIParameter
	{
		string GetString();
	}

	/// <summary>
	/// Summary for ClassWithDIConstructor
	/// </summary>
	[NDOPersistent]
	public partial class ClassWithDIConstructor
	{
		string persistentField;
		[NDOTransient]
		string diField;


		/// <summary>
		/// This constructory can't be used by DI,
		/// so NDO should use the other constructor.
		/// </summary>
		/// <param name="parameter"></param>
		public ClassWithDIConstructor( string s )
		{
			this.diField = s;
		}

		public ClassWithDIConstructor(IDIParameter parameter)
		{
			this.diField = parameter.GetString();
		}

		public string PersistentField
		{
			get { return this.persistentField; }
			set { this.persistentField = value; }
		}

		public string DiField
		{
			get { return this.diField; }
		}
	}
}
