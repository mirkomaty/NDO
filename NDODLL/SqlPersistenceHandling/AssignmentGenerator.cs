using NDO.Mapping;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NDO.SqlPersistenceHandling
{
	internal class AssignmentGenerator
	{
		IProvider provider;
		List<string> fields = new List<string>();

		public AssignmentGenerator( Class cls )
		{
			this.provider = cls.Provider;
			FieldMap fm = new FieldMap( cls );
			foreach (var e in fm.PersistentFields)
			{
				if (e.Value.CustomAttributes.Any( c => c.AttributeType == typeof( NDOReadOnlyAttribute ) ))
					continue;

				var fieldMapping = cls.FindField( (string)e.Key );
				if (fieldMapping != null)
				{
					fields.Add( fieldMapping.Column.Name );
				}
			}

			var relationInfos = new RelationCollector( cls ).CollectRelations().ToList();

			foreach (RelationFieldInfo ri in relationInfos)
			{
				Relation r = ri.Rel;
				if (r.Multiplicity == RelationMultiplicity.Element && r.MappingTable == null
					|| r.Multiplicity == RelationMultiplicity.List && r.MappingTable == null && r.Parent.FullName != cls.FullName)
				{
					foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
					{
						fields.Add( fkColumn.Name );
						Type systemType = fkColumn.SystemType;
					}
					if (r.ForeignKeyTypeColumnName != null)
					{
						fields.Add( r.ForeignKeyTypeColumnName );
					}
				}
			}
			if (cls.TimeStampColumn != null)
			{
				fields.Add( cls.TimeStampColumn );
			}

		}

		public string Result
		{
			get
			{
				StringBuilder result = new StringBuilder();
				int ende = fields.Count - 1;
				for (int i = 0; i < fields.Count; i++)
				{
					string fieldName = (string)fields[i];
					result.Append( provider.GetQuotedName( fieldName ) );
					result.Append( " = " );
					if (!provider.UseNamedParams)
						result.Append( "?" );
					else
					{
						result.Append( $"{{{i}}}" );
					}
					if (i < ende)
					{
						result.Append( ", " );
					}
				}
				return result.ToString();
			}
		}
	}
}
