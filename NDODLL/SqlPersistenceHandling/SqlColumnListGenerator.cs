using NDO.Mapping;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NDO.SqlPersistenceHandling
{
	/// <summary>
	/// Helper class to generate lists of columns which can be used in Sql queries
	/// </summary>
	public class SqlColumnListGenerator
	{
		IProvider provider;
		List<string> baseColumnList = new List<string>();
		List<string> selectColumnList = new List<string>();
		List<string> paramList = new List<string>();
		List<string> hollowList = new List<string>();

		string tableName;
		Type resultType;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cls"></param>
		public SqlColumnListGenerator( Class cls )
		{
			Init( cls );
		}

		/// <summary>
		/// Initializes the ColumnListGenerator
		/// </summary>
		/// <param name="cls"></param>
		public void Init( Class cls )
		{
			this.tableName = cls.TableName;
			this.resultType = cls.SystemType;

			provider = cls.Provider;
			FieldMap fm = new FieldMap( cls );
			var persistentFields = fm.PersistentFields;
			var relationInfos = new RelationCollector( cls ).CollectRelations().ToList();
			List<string> readOnlyColumns = new List<string>();

			foreach (OidColumn oidColumn in cls.Oid.OidColumns)
			{
				hollowList.Add( oidColumn.Name );

				Relation r = oidColumn.Relation;
				if (r != null && r.ForeignKeyTypeColumnName != null)
					hollowList.Add( r.ForeignKeyTypeColumnName );  // Die Frage ist, ob das nicht auch in der baseColumnList auftauchen muss.

				if (!oidColumn.AutoIncremented && oidColumn.FieldName == null && oidColumn.RelationName == null)
				{
					baseColumnList.Add( oidColumn.Name );
					paramList.Add( oidColumn.Name );
				}
			}

			foreach (var e in persistentFields)
			{
				Type memberType;
				if (e.Value is FieldInfo)
					memberType = ((FieldInfo)e.Value).FieldType;
				else
					memberType = ((PropertyInfo)e.Value).PropertyType;

				var fieldMapping = cls.FindField( (string)e.Key );
				bool isReadOnly = false;

				if (e.Value.CustomAttributes.Any( c => c.AttributeType == typeof( NDOReadOnlyAttribute ) ))
				{
					readOnlyColumns.Add( fieldMapping.Column.Name );
					isReadOnly = true;
				}

				// Ignore fields without mapping.
				if (null == fieldMapping)
					continue;

				if (null == fieldMapping.Column.DbType)
				{
					fieldMapping.ColumnDbType = (int)provider.GetDbType( memberType );
				}
				else
				{
					fieldMapping.ColumnDbType = (int)provider.GetDbType( fieldMapping.Column.DbType );
				}

				if (!isReadOnly)
				{
					baseColumnList.Add( fieldMapping.Column.Name );
					paramList.Add( fieldMapping.Column.Name );
				}
			}

			foreach (RelationFieldInfo ri in relationInfos)
			{
				Relation r = ri.Rel;
				foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
				{
					baseColumnList.Add( fkColumn.Name );
					paramList.Add( fkColumn.Name );
				}
				if (r.ForeignKeyTypeColumnName != null)
				{
					baseColumnList.Add( r.ForeignKeyTypeColumnName );
					paramList.Add( r.ForeignKeyTypeColumnName );
				}

			}

			if (cls.TimeStampColumn != null)
			{
				baseColumnList.Add( cls.TimeStampColumn );
				paramList.Add( cls.TimeStampColumn );
				hollowList.Add( cls.TimeStampColumn );
			}

			if (cls.TypeNameColumn != null)
			{
				var typeColumnName = cls.TypeNameColumn.Name;
				baseColumnList.Add( typeColumnName );
				paramList.Add( typeColumnName );
				hollowList.Add( typeColumnName );
			}

			foreach (OidColumn oidColumn in cls.Oid.OidColumns)
			{
				if (oidColumn.FieldName == null && oidColumn.RelationName == null && oidColumn.AutoIncremented)
					selectColumnList.Add( oidColumn.Name );
			}

			selectColumnList.AddRange( baseColumnList );
			selectColumnList.AddRange( readOnlyColumns );

		}

		/// <summary/>
		public string BaseList => Result( this.baseColumnList, false, false );
		/// <summary/>
		public string SelectList => Result( this.selectColumnList, false, false );
		/// <summary/>
		public string SelectListWithAlias => Result( this.selectColumnList, true, false );
		/// <summary/>
		public string HollowFields => Result( this.hollowList, false, false );
		/// <summary/>
		public string HollowFieldsWithAlias => Result( this.hollowList, true, false );

		/// <summary/>
		public string ParamList
		{
			get
			{
				StringBuilder result = new StringBuilder();
				int ende = this.paramList.Count - 1;
				for (int i = 0; i < this.paramList.Count; i++)
				{
					if (this.provider.UseNamedParams)
					{
						result.Append( $"{{{i}}}" );
						if (i < ende)
						{
							result.Append( ", " );
						}
					}
					else
					{
						if (i < ende)
						{
							result.Append( "?, " );
						}
						else
							result.Append( "?" );
					}
				}

				return result.ToString();
			}
		}

		/// <summary/>
		public string Result(bool hollow, bool generateAliasNames, bool useTableName)
		{
			return Result( hollow ? this.hollowList : this.selectColumnList, generateAliasNames, useTableName );
		}

		string Result(List<string> columnList, bool generateAliasNames, bool useTableName )
		{
			StringBuilder sb = new StringBuilder();
			int ende = columnList.Count - 1;
			for (int i = 0; i < columnList.Count; i++)
			{
				string f = columnList[i];
				if (useTableName)
				{
					sb.Append( provider.GetQualifiedTableName( tableName ) );
					sb.Append( '.' );
				}
				sb.Append( provider.GetQuotedName( f ) );
				if (generateAliasNames)
				{
					sb.Append( " AS " );
					sb.Append( provider.GetQuotedName( f ) );
				}
				if (i < ende)
				{
					sb.Append( ", " );
				}
			}

			return sb.ToString();
		}
	}
}
