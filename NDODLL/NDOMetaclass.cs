﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using NDO.Configuration;

namespace NDO
{
	/// <summary>
	/// This class wraps the MetaClass generated by old versions of the NDOEnhancer,
	/// in order to support resolving constructor parameters.
	/// </summary>
	class NDOMetaclass : MetaclassBase
	{
		private readonly IMetaClass innerClass;

		public NDOMetaclass(Type t, IMetaClass innerClass) : base(t)
		{
			this.innerClass = innerClass;
		}

		/// <inheritdoc/>
		public override IPersistenceCapable CreateObject()
		{
			//if (IsDefaultConstructor)
			return innerClass.CreateObject();
			//throw new Exception( "Can't use CreateObject() if the constructor expects parameters. Use CreateObject(INDOContainer configContainer) instead." );
		}

		/// <inheritdoc/>
		public override int GetRelationOrdinal( string fieldName )
		{
			return innerClass.GetRelationOrdinal( fieldName );
		}
	}
}
