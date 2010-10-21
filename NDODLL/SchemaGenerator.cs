//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Reflection;
using System.Data;
using System.Collections;
using NDO;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO
{

	/// <summary>
	/// SchemaGenerator.
	/// </summary>
	internal class SchemaGenerator
	{
		public SchemaGenerator(Class cl, NDOMapping mappings, DataSet dsSchema)
		{
			this.mappings = mappings;
			this.dsSchema = dsSchema;
			this.c = cl;
			FieldMap fieldMap = new FieldMap(cl);
			this.persistentFields = fieldMap.PersistentFields;
			this.supportsGuidType = CheckGuidType();
		}
		Hashtable persistentFields;
		NDOMapping mappings;
		DataSet dsSchema;
		Class c;
		bool supportsGuidType;
//		ArrayList baseNames = null;

		public void GenerateRelations()
		{
			DataTable dt = dsSchema.Tables[c.TableName];
			if (null == dt)
				throw new InternalException(36, String.Format("Schemagenerator: GenerateRelations: DataTable {0} not found.", c.TableName));

			foreach(Relation r in c.Relations)
			{
				//TODO: Checken, ob man hier nicht abbrechen muss
//				if ((r.AllowSubclasses) != 0)
//					continue;

				string rname = r.FieldName;

				if (r.MappingTable != null)
					continue;

				NDO.Mapping.Class refClass = this.mappings.FindClass(r.ReferencedTypeName);
				if (null == refClass)
					throw new InternalException(51, String.Format("SchemaGenerator: Reference Class {0} not found", r.ReferencedTypeName));

				// Kann keine Relation zu einer abstrakten Klasse herstellen,
				// da diese keine Tabelle hat
				if (refClass.IsAbstract)
					continue;
					
				//MM 5.2 Relationen anlegen 
				// Zwei gegenläufige 1:1-Beziehungen benötigen zwei Relationen
				DataColumn parentCol;
				DataColumn childCol;
				DataTable foreignTable = dsSchema.Tables[refClass.TableName];
				if (foreignTable == null)
					throw new InternalException(64, String.Format("SchemaGenerator: GenerateRelations: foreign Table {0} for class {1} not found.", refClass.TableName, refClass.FullName));

				string relName;
				if (r.Multiplicity == RelationMultiplicity.Element && GegenRelationIs1To1(r))
				{
					relName = dt.TableName + refClass.TableName + r.RelationName;
				}
				else
				{
					if (refClass.TableName.CompareTo(dt.TableName) < 0)
						relName = refClass.TableName + dt.TableName + r.RelationName;
					else
						relName = dt.TableName + refClass.TableName + r.RelationName;
				}

                int colIndex = 0;
#if DEBUG
                if (r.Multiplicity == RelationMultiplicity.Element)
                    System.Diagnostics.Debug.Assert(refClass.Oid.OidColumns.Count == r.ForeignKeyColumns.Count);
                else
                    System.Diagnostics.Debug.Assert(c.Oid.OidColumns.Count == r.ForeignKeyColumns.Count);
#endif
                new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                {
                    if (r.Multiplicity == RelationMultiplicity.Element)
                    {
                        if (refClass.Oid.IsDependent)
                            throw new NDOException(36, String.Format("Wrong Relation '{0}.{1}'. Relations to intermediate classes are not allowed.", c.FullName, r.FieldName));
                        childCol = dt.Columns[fkColumn.Name];
                        if (childCol == null)
                            throw new InternalException(82, String.Format("SchemaGenerator: GenerateRelations: Column {0}.{1} not found", dt.TableName, fkColumn.Name));
                        string oidColumnName = ((OidColumn)refClass.Oid.OidColumns[colIndex]).Name;
                        parentCol = foreignTable.Columns[oidColumnName];
                        if (parentCol == null)
                            throw new InternalException(85, String.Format("SchemaGenerator: GenerateRelations: Column {0}.{1} is null", foreignTable.TableName, oidColumnName));
                    }
                    else
                    {
                        if (c.Oid.IsDependent)
                            throw new NDOException(36, String.Format("Wrong Relation '{0}.{1}'. Relations to intermediate classes are not allowed.", c.FullName, r.FieldName));
                        childCol = foreignTable.Columns[fkColumn.Name];
                        if (childCol == null)
                            throw new InternalException(91, String.Format("SchemaGenerator: GenerateRelations: Column {0}.{1} not found", foreignTable.TableName, fkColumn.Name));
                        string oidColumnName = ((OidColumn)c.Oid.OidColumns[colIndex]).Name;
                        parentCol = dt.Columns[oidColumnName];
                        if (parentCol == null)
                            throw new InternalException(94, String.Format("SchemaGenerator: GenerateRelations: Column {0}.{1} ist null", dt.TableName, oidColumnName));
                    }
                    if (null == dsSchema.Relations[relName])
                    {
                        DataRelation dr = new DataRelation(relName, parentCol, childCol, true);
                        dsSchema.Relations.Add(dr);
                        ForeignKeyConstraint fkc = dr.ChildKeyConstraint;
                        fkc.UpdateRule = Rule.Cascade;
                        fkc.DeleteRule = Rule.None;
                        fkc.AcceptRejectRule = AcceptRejectRule.None;
                    }
                    colIndex++;
                });  // End delegate

			}
		}


		private bool CheckGuidType()
		{
            // Since the generator might be called from the enhancer
            // with no provider defined, we have can't use GetProvider
            // because it throws an exeption, if no provider exists.
            //IProvider provider = mappings.GetProvider(c);

            Connection conn = mappings.FindConnection(c);
            if (conn == null)
                throw new NDOException(102, "No connection defined in class mapping for type " + c.FullName + ". Check your mapping file.");
            if (conn.Type == string.Empty)
                return true;
            IProvider provider = NDOProviderFactory.Instance[conn.Type];
            if (provider == null)
                return true;
            return (provider.SupportsNativeGuidType);
		}



		public void GenerateTables()
		{
			string pureName = c.FullName;
			//MM 1 DataTable anlegen
			DataTable dt = dsSchema.Tables[c.TableName];
			if (null == dt)
			{
				dt = new DataTable(c.TableName);
				dsSchema.Tables.Add(dt);
			}
			
			if (!c.Oid.IsDependent)  // Columns will be built as foreign key columns of the relations
			{
                ArrayList primaryKeyColumns = new ArrayList();
                new OidColumnIterator(c).Iterate(delegate(OidColumn oidColumnMapping, bool isLastElement)
                {
                    //MM 3 Oid-Spalte anlegen
                    DataColumn oidDataColumn = SearchAndAddColumn(dt, oidColumnMapping.Name, oidColumnMapping.SystemType);
                    // The name might have been changed while generating the column, so the mapping has to change
                    oidColumnMapping.Name = oidDataColumn.ColumnName;

                    oidDataColumn.AllowDBNull = false;
                    oidDataColumn.Unique = c.Oid.OidColumns.Count == 1;

                    //Wenn es einen Hint gibt [NDOOidType(typeof(int))], dann muss
                    // AutoIncrement auf false stehen.			
                    //bool autoIncrement = c.SystemType.GetCustomAttributes(typeof(NDOOidTypeAttribute), true).Length == 0;

                    //MM 3.1 Auto-Inkrement-Parameter setzen
                    if (oidColumnMapping.AutoIncremented && oidDataColumn.DataType == typeof(int))
                    {
                        oidDataColumn.AutoIncrement = true;
                        oidDataColumn.AutoIncrementSeed = -(oidColumnMapping.AutoIncrementStart);
                        oidDataColumn.AutoIncrementStep = -(oidColumnMapping.AutoIncrementStep);
                    }

                    //MM 3.2 Primary Key der Tabelle auf die Oid-Spalte setzen
                    primaryKeyColumns.Add(oidDataColumn);
                }); // End Delegate
                DataColumn[] oidColums = new DataColumn[primaryKeyColumns.Count];
                primaryKeyColumns.CopyTo(oidColums);
                dt.PrimaryKey = oidColums;
            }
			string fname;

			//MM 3.3 Falls eine TimeStamp-Spalte existiert, diese anlegen
			//Timestamps sind immer vom Typ Guid
			if (c.TimeStampColumn != null)
				SearchAndAddColumn(dt, c.TimeStampColumn, typeof(Guid));

            //MM 3.4 If a TypeNameColumn exists in the mapping, create it
            if (c.TypeNameColumn != null)
            {
                DataColumn tnc = SearchAndAddColumn(dt, c.TypeNameColumn.Name, Type.GetType(c.TypeNameColumn.NetType));
                tnc.AllowDBNull = false;
            }


			//MM 4 Felder anlegen
			foreach(Field f in c.Fields)
			{
				fname = (string) f.Name;
//				string[] strarr = fname.Split('.');
//				string currentFieldName = string.Empty;
//				Type parentType = c.SystemType;
//				FieldInfo fi = null;
//				for(int i = 0; i < strarr.Length; i++)
//				{
//					currentFieldName += strarr[i];
//					fi = parentType.GetField(strarr[i], BindingFlags.NonPublic | BindingFlags.Instance);
//					if (null == fi)
//					{
//						throw new NDOExceptggion("Class " + pureName + " doesn't hava a field '" + currentFieldName + "'. Check your mapping file.");
//						break;
//					}
//					if (i < strarr.Length - 1)
//					{
//						currentFieldName += '.';
//						parentType = fi.FieldType;
//					}
//				}
//				if (fi == null)
//					throw new Exception("Field Type not found");
					//continue;
				object memberInfo = this.persistentFields[fname];
				if (memberInfo == null)  // protected inherited field
					continue;
				Type t;
				if (memberInfo is FieldInfo)
					t = ((FieldInfo)memberInfo).FieldType;
				else
					t = ((PropertyInfo)memberInfo).PropertyType;
				// Don't use f.ColumnType here. This is used
				// for the definition of special Sql data types.
				SearchAndAddColumn(dt, f.Column.Name, t, f.Column.Size);
			}


			//MM 4 Relations anlegen
			//MM 4.1 Relations anlegen - 1:1 und solche mit Mapping Table

			foreach(Relation r in c.Relations)
			{
				string rname = r.FieldName;

				NDO.Mapping.Class refClass = this.mappings.FindClass(r.ReferencedType);
				if (null == refClass)
				{
					throw new InternalException(219, "SchemaGenerator.");
//					messages.WriteLine("Fehler im Schemagenerator: Kann Mapping-Information für Klasse " + ri.RefType + " nicht finden");
//					messages.WriteInsertedLine("Referenzierte Klasse muss persistent sein");
//					continue;
				}
				
				//MM 5 ggf. MappingTable anlegen
				// Gibt's ne MappingTable in ri?
				if (null != r.MappingTable)
				{
					GenerateMappingTable(c, refClass, r, dt);
				}
				else // Keine Mapping-Tabelle
				{
					//MM 5.1 wenns keine Mapping Table gibt: Foreign Key Spalten anlegen
					//MM 5.1.1 1:1 - FK ist in eigener Tabelle
					//MM 5.1.2 nicht 1:1 - FK ist in fremder Tabelle
					//MM 5.1.2 dies soll die Gegenklasse erledigen
				
					if (r.Multiplicity == RelationMultiplicity.Element)
					{
                        new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                        {
						    //    Wenn noch nicht vorhanden:
						    //    Lege Spalte r.ForeignKeyColumnName in dt an
						    SearchAndAddColumn(dt, fkColumn.Name, fkColumn.SystemType);
                        }); // End Delegate
                        if (r.ForeignKeyTypeColumnName != null)
                            SearchAndAddColumn(dt, r.ForeignKeyTypeColumnName, typeof(int));
                    }
				}				
			}

			//MM 5.1.2 nicht 1:1 - FK ist in fremder Tabelle
			//wir legen nicht die Fremdschlüssel in der fremden Tabelle an, sondern
			//die Schlüssel fremder Relationen in der eigenen Tabelle
			foreach (NDO.Mapping.Class cl in mappings.Classes)
			{
				foreach(Relation r in cl.Relations)
				{
					if (r.MappingTable == null && r.ReferencedType.IsAssignableFrom(c.SystemType))
					{
						if (r.Multiplicity == RelationMultiplicity.List)
						{
                            new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                            {

                                // 1:n-Relation auf unsere Klasse - Schlüssel muss bei uns rein							
                                SearchAndAddColumn(dt, fkColumn.Name, fkColumn.SystemType);
                            });

                            // Wir müssen auf die TypeColumn Rücksicht nehmen.
                            // Das ist die Umkehrung einer 1:n-Beziehung, wir befinden
                            // uns auf der 1-Seite. Ist die 1-Seite polymorph, entspricht
                            // das einer polymorphen Element-Beziehung.
                            if (r.ForeignKeyTypeColumnName != null)
                                SearchAndAddColumn(dt, r.ForeignKeyTypeColumnName, typeof(int));
                        }
					}
				}
			}
		}


		private DataColumn SearchAndAddColumn( DataTable dt, string colName, Type t )
		{
			return SearchAndAddColumn(dt, colName, t, 0);
		}

		private DataColumn SearchAndAddColumn(DataTable dt, string colName, Type t, int colLength)
		{
			if (null != dt.Columns[colName])
				return dt.Columns[colName];

			Type t2 = t;
            if (t.IsGenericParameter)
                t2 = typeof(string);
            else if (t.FullName.StartsWith("System.Nullable`1"))
                t2 = t.GetGenericArguments()[0];
            if (t2 == typeof(Guid) && !supportsGuidType)
                t2 = typeof(string);
            else if (t2.IsEnum)
                t2 = Enum.GetUnderlyingType(t2);
			DataColumn dc = new DataColumn(colName, t2);
			dc.AllowDBNull = true;
			if (colLength != 0)
			{
				if (t2 == typeof(string))
					dc.MaxLength = colLength;
				dc.ExtendedProperties.Add("MaxLength", colLength);
			}
			dt.Columns.Add(dc);

			return dc;	
		}

		private void GenerateMappingTable(NDO.Mapping.Class parentClass, NDO.Mapping.Class relClass, NDO.Mapping.Relation r, DataTable parentTable)
		{
			//MM 6 ggf. Tabelle anlegen
			//    Gibt's die Table schon in dsSchema?
			//    Nein: Anlegen
			DataTable dtMap = dsSchema.Tables[r.MappingTable.TableName];
			if (null == dtMap)
				dsSchema.Tables.Add(dtMap = new DataTable(r.MappingTable.TableName));

			//MM 7 ForeignKey-Spalten für eigene und Fremdklasse anlegen
			//MM 7 Wenn nötig auch die TypeColumns anlegen
			//    Addiere die Spalte r.ForeignKeyColumnName
            new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
            {
                SearchAndAddColumn(dtMap, fkColumn.Name, fkColumn.SystemType);
            });
			//	Lege ggf. die Typspalte an, Typ ist immer int
			if (r.ForeignKeyTypeColumnName != null)
				SearchAndAddColumn(dtMap, r.ForeignKeyTypeColumnName, typeof(int));

			// Addiere die Spalte r.MappingTable.ChildForeignKeyColumnName
            new ForeignKeyIterator(r.MappingTable).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
            {
                SearchAndAddColumn(dtMap, fkColumn.Name, fkColumn.SystemType);
            });
#if nix  // Old single column code
			Type defaultType = null;

			//TODO: Interfaces berücksichtigen
			if (relClass == null)  // Interface
			{
				// Typ ist Interface
				throw new NotImplementedException("Interface");
			}
			else
			{
				defaultType = relClass.Oid.FieldType;
			}
			searchAndAddColumn(dtMap, r.MappingTable.ChildForeignKeyColumnName, defaultType);
#endif
			if (r.MappingTable.ChildForeignKeyTypeColumnName != null)
				SearchAndAddColumn(dtMap, r.MappingTable.ChildForeignKeyTypeColumnName, typeof(int));

		}


		private bool GegenRelationIs1To1(Relation r)
		{
			return (r.ForeignRelation != null && r.ForeignRelation.Multiplicity == RelationMultiplicity.Element);
		}

	}
}
