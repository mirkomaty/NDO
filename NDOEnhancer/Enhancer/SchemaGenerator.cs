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


#if nix  // Since 2.0 the Generator in the NDO.dll is used
using System;
using System.Xml;
using System.Data;
using System.Collections;
using NDO;
using NDO.Mapping;

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für SchemaGenerator.
	/// </summary>
	internal class SchemaGenerator
	{
		public SchemaGenerator(Class c, NDOMapping mappings, DataSet dsSchema, MessageAdapter messages, Hashtable allSortedFields, ClassHashtable allPersistentClasses, bool verboseMode)
		{
			this.mappings = mappings;
			this.messages = messages;
			this.dsSchema = dsSchema;
			this.allSortedFields = allSortedFields;
			this.c = c;
			this.allPersistentClasses = allPersistentClasses;
            this.verboseMode = verboseMode;
		}
		NDOMapping mappings;
		MessageAdapter messages;
		DataSet dsSchema;
		Hashtable allSortedFields;
		ClassHashtable allPersistentClasses;
		Class c;
		ArrayList baseNames = null;
        bool verboseMode;

		public string GetLastElement(string input)
		{
			string[] arr = input.Split('.');
			return arr[arr.Length - 1];
		}

		public void GenerateRelations()
		{
			DataTable dt = dsSchema.Tables[c.TableName];
			if (null == dt)
				throw new InternalException(38, String.Format("GenerateRelations: DataTable {0} not found.", c.TableName));

			ClassNode classNode = (ClassNode)allPersistentClasses[c.FullName];
			if (classNode == null)
				throw new InternalException(42, "SchemaGenerator: GenerateRelations");

//			ReferenceArrayList references = (ReferenceArrayList) allReferences[c.FullName];
//			if (references == null)
//				throw new NDOException(string.Format("Kann Relationsinformationen für die Klasse {0} nicht finden. Rekompilieren Sie ggf. das Assembly neu", c.FullName));
//
			foreach(Relation r in c.Relations)
			{
				string rname = r.FieldName;
				if (null == r)
				{
					messages.WriteLine("Keine Mapping-Informationen für Relation " + c.FullName + "." + rname + ". Lassen Sie sich die Information generieren oder tragen Sie sie per Hand ein.");
					continue;
				}

				if (r.MappingTable != null)
					continue;

				NDO.Mapping.Class refClass = this.mappings.FindClass(r.ReferencedTypeName);
				if (null == refClass)
					throw new InternalException(64, String.Format("Reference Class {0} nicht gefunden", r.ReferencedTypeName));

				ClassNode refClassNode = (ClassNode)allPersistentClasses[refClass.FullName];

				// Kann keine Relation zu einer abstrakten Klasse herstellen,
				// da diese keine Tabelle hat
				if (refClassNode.IsAbstract)
					continue;
					
				//MM 5.2 Relationen anlegen 
				// Zwei gegenläufige 1:1-Beziehungen benötigen zwei Relationen
				DataColumn parentCol;
				DataColumn childCol;
				DataTable foreignTable = dsSchema.Tables[refClass.TableName];
				if (foreignTable == null)
					throw new InternalException(77, String.Format("SchemaGenerator: GenerateRelations: foreign Table {0} for class {1} not found.", refClass.TableName, refClass.FullName));

				string relName;
				if (r.Multiplicity == RelationMultiplicity.Element && GegenRelationIs1To1(c.FullName, refClassNode, r))
				{
					relName = GetLastElement(dt.TableName) + GetLastElement(refClass.TableName) + r.RelationName;
				}
				else
				{
					if (refClass.TableName.CompareTo(dt.TableName) < 0)
						relName = GetLastElement(refClass.TableName) + GetLastElement(dt.TableName) + r.RelationName;
					else
						relName = GetLastElement(dt.TableName) + GetLastElement(refClass.TableName) + r.RelationName;
				}
                int i = 0;
                new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                {
                    if (r.Multiplicity == RelationMultiplicity.Element)
                    {
                        childCol = dt.Columns[fkColumn.Name];
                        if (childCol == null)
                            throw new Exception(String.Format("Internal Error: GenerateRelations: Column {0}.{1} not found", dt.TableName, fkColumn.Name));
                        string oidColumnName = ((OidColumn)refClass.Oid.OidColumns[i]).Name;
                        parentCol = foreignTable.Columns[oidColumnName];
                        if (parentCol == null)
                            throw new Exception(String.Format("Internal Error: GenerateRelations: Column {0}.{1} is null", foreignTable.TableName, oidColumnName));
                    }
                    else
                    {
                        childCol = foreignTable.Columns[fkColumn.Name];
                        if (childCol == null)
                            throw new Exception(String.Format("Internal Error: GenerateRelations: Column {0}.{1} not found", foreignTable.TableName, fkColumn.Name));
                        string oidColumnName = ((OidColumn)c.Oid.OidColumns[i]).Name;
                        parentCol = dt.Columns[oidColumnName];
                        if (parentCol == null)
                            throw new Exception(String.Format("Internal Error: GenerateRelations: Column {0}.{1} is null", dt.TableName, oidColumnName));
                    }
                    if (null == dsSchema.Relations[relName])
                    {
                        try
                        {
                            DataRelation dr = new DataRelation(relName, parentCol, childCol, true);
                            dsSchema.Relations.Add(dr);
                            ForeignKeyConstraint fkc = dr.ChildKeyConstraint;
                            fkc.UpdateRule = Rule.Cascade;
                            fkc.DeleteRule = Rule.None;
                            fkc.AcceptRejectRule = AcceptRejectRule.None;
                        }
                        catch (Exception ex)
                        {
                            if (verboseMode)
                                messages.ShowError("Error while constructing relation " + relName + ": " + ex.ToString());
                            else
                                messages.ShowError("Error while constructing relation " + relName + ": " + ex.Message);

                        }
                    }
                    i++;
                });
			}

#if !NDO20
#if BETA
			if (!NDOKey.CheckKey(new Guid("889b6a58-1d97-be54-d874-75a12a93f56e"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if TRIAL  
			if (!NDOKey.CheckKey(new Guid("7f63d54d-d500-228e-311d-b19bb99d9bd4"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if ENT
			if (!NDOKey.CheckKey(new Guid("9d092588-f787-cbed-fe06-2dcd86720337"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if PRO
			if (!NDOKey.CheckKey(new Guid("ed25a316-3365-1e4e-5144-f3a1766fe98d"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if STD
			if (!NDOKey.CheckKey(new Guid("ec3be943-c358-b83a-e310-b8436322e322"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#endif
#endif
#endif
#endif
#endif
#else  // NDO20
#if BETA
			if (!NDOKey.CheckKey(new Guid("2f29c228-6967-4941-02dd-369783fabdde"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if TRIAL  
			if (!NDOKey.CheckKey(new Guid("629dab4d-a032-a30e-54ea-1a53e0111cb9"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if ENT
            if (!NDOKey.CheckKey(new Guid("9540866a-861c-3f19-ae8c-7f9785f62ce5"),
                new LicenceKey().TheKey))
                throw new WrongLicenceException2();
#else
#if PRO
			if (!NDOKey.CheckKey(new Guid("35473777-9fc3-f78f-83c6-475f23758b46"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#else
#if STD
			if (!NDOKey.CheckKey(new Guid("36bf154d-4085-9f38-d2a9-1fea6a6f9df8"), 
				new LicenceKey().TheKey))
				throw new WrongLicenceException2();
#endif
#endif
#endif
#endif
#endif
#endif // NDO20

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
			
			//MM 2 Name des Oid-Fields bestimmen
			if (!c.Oid.IsDependent)
			{
                ArrayList primaryKeyColumns = new ArrayList();
                new OidColumnIterator(c).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    //MM 3 Oid-Spalte anlegen
                    DataColumn oidDataColumn = searchAndAddColumn(dt, oidColumn.Name, null, oidColumn.SystemType);
                    // Der Name kann sich beim Anlegen geändert haben
                    oidColumn.Name = oidDataColumn.ColumnName;

                    oidDataColumn.AllowDBNull = false;
                    oidDataColumn.Unique = true;

                    //Wenn es einen Hint gibt [NDOOidType(typeof(int))], dann muss
                    // AutoIncrement auf false stehen.
                    //ClassNode classNode = (ClassNode)allPersistentClasses[c.FullName];
                    //if (classNode == null)
                    //    throw new InternalException(170, "SchemaGenerator");

                    //bool autoIncrement = true;
                    //autoIncrement = classNode.OidType == null;


                    //MM 3.1 Auto-Increment-Parameter setzen
                    if (oidColumn.AutoIncremented)
                    {
                        oidDataColumn.AutoIncrement = true;
                        oidDataColumn.AutoIncrementSeed = -1;
                        oidDataColumn.AutoIncrementStep = -1;
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
			//Timestamps sind immer vom Typ byte[]
			if (c.TimeStampColumn != null)
				searchAndAddColumn(dt, c.TimeStampColumn, null, typeof(Guid));

            //MM 3.4 Falls eine TypeNameColumn existiert, eine anlegen
            if (c.TypeNameColumn != null)
            {
                DataColumn tnc = searchAndAddColumn(dt, c.TypeNameColumn.Name, null, Type.GetType(c.TypeNameColumn.NetType));
                tnc.AllowDBNull = false;
            }

			//MM 4 Felder anlegen
			IList sortedFields = (IList) allSortedFields[pureName];
			if (sortedFields != null) // other assembly
			{
				foreach(DictionaryEntry de in sortedFields)
				{
					fname = (string) de.Key;
					Patcher.ILField fi = (Patcher.ILField) de.Value;

					NDO.Mapping.Field f = c.FindField(fname);
					if (null == f)
					{
						messages.WriteLine("Keine Mapping-Informationen für Feld " + pureName + "." + fname + ". Lassen Sie sich die Information generieren oder tragen Sie sie per Hand ein.");
						continue;
					}
					// Hier wird nicht der f.ColumnType verwendet. Dieser ist für
					// spezielle Sql-Datentypen gedacht
					searchAndAddColumn(dt, f.Column.Name, fi.IsEnum ? "System.Int32" : fi.CsType, typeof(string));
				}
			}

			//MM 4 Relations anlegen
			//MM 4.1 Relations anlegen - 1:1 und solche mit Mapping Table
//			ReferenceArrayList references = (ReferenceArrayList) allReferences[pureName];
//			if (references == null)
//				throw new NDOException(string.Format("Kann Relationsinformationen für die Klasse {0} nicht finden. Rekompilieren Sie ggf. das Assembly neu", pureName));

			foreach(Relation r in c.Relations)
			{
				string rname = r.FieldName;
//				NDO.Mapping.Relation r = c.FindRelation(rname);
				if (null == r)
				{
					messages.WriteLine("Keine Mapping-Informationen für Relation " + pureName + "." + rname + ". Lassen Sie sich die Information generieren oder tragen Sie sie per Hand ein.");
					continue;
				}

				NDO.Mapping.Class refClass = this.mappings.FindClass(r.ReferencedTypeName);
				if (null == refClass)
				{
					throw new Exception("Can't find class mapping for type " + r.ReferencedTypeName + ". Check your mapping file or merge the mapping information from your base assemblies.");
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

						//    Wenn noch nicht vorhanden:
						//    Lege Spalte r.ForeignKeyColumnName in dt an
                        new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                        {
                            searchAndAddColumn(dt, fkColumn.Name, null, fkColumn.SystemType);
                        });

						if (r.ForeignKeyTypeColumnName != null)
							searchAndAddColumn(dt, r.ForeignKeyTypeColumnName, null, typeof(int));
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
					if (r.MappingTable == null && ReferencedNameIsAssignable(r.ReferencedTypeName))
					{
						if (r.Multiplicity == RelationMultiplicity.List)
						{
                            new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
                            {
                                // 1:n-Relation auf unsere Klasse - Schlüssel muss bei uns rein							
                                searchAndAddColumn(dt, fkColumn.Name, null, fkColumn.SystemType);
                            });

							// Wir müssen auf die TypeColumn Rücksicht nehmen.
							// Das ist die Umkehrung einer 1:n-Beziehung, wir befinden
							// uns auf der 1-Seite. Ist die 1-Seite polymorph, entspricht
							// das einer polymorphen Element-Beziehung.
							if (r.ForeignKeyTypeColumnName != null)
								searchAndAddColumn(dt, r.ForeignKeyTypeColumnName, null, typeof(int));
						}
					}
				}
			}
		}



		private DataColumn searchAndAddColumn(DataTable dt, string colName, string typeName, Type defaultType)
		{
			if (null != dt.Columns[colName])
				return dt.Columns[colName];

			DataColumn colOid = new DataColumn(colName);

			Type t = null;
			if (null != typeName)
			{
                if (typeName.StartsWith("!"))
                    t = typeof(string);
                else
				    t = Type.GetType(typeName);
			}
			if (null == t)
			{
				t = defaultType;
			}
			if (null == t)
			{
				messages.WriteLine("Table " + dt.TableName + ": Can't determine column type for column " + colName + ". ");
				messages.WriteInsertedLine("Trying to get away with System.String.");
				t = typeof(string);
			}

			DataColumn dc = new DataColumn(colName, t);
			dc.AllowDBNull = true;
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
			//    Addiere die Spalte r.ForeignKeyColumnName, Typ der Spalte: parentClass.Oid.ColumnType bzw. int
            new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
            {
                searchAndAddColumn(dtMap, fkColumn.Name, null, fkColumn.SystemType);
            });

			//	Lege ggf. die Typspalte an, Typ ist immer int
			if (r.ForeignKeyTypeColumnName != null)
				searchAndAddColumn(dtMap, r.ForeignKeyTypeColumnName, null, typeof(int));

            new ForeignKeyIterator(r.MappingTable).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastElement)
            {
                searchAndAddColumn(dtMap, fkColumn.Name, null, fkColumn.SystemType);
            });

#if nix // Single oid column code
			// Addiere die Spalte r.MappingTable.ChildForeignKeyColumnName
			// Typ der Spalte muss ermittelt werden:
			// - Oid.FieldType
			// - [NDOObjectId] für Interfaces
			string typeName = null;
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
#endif
			if (r.MappingTable.ChildForeignKeyTypeColumnName != null)
				searchAndAddColumn(dtMap, r.MappingTable.ChildForeignKeyTypeColumnName, null, typeof(int));

		}

		public ArrayList BaseNames
		{
			get 
			{
				if (baseNames == null)
				{		
					baseNames = new ArrayList();
					string className = c.FullName;
					ClassNode classNode;
					while ((classNode = (ClassNode)allPersistentClasses[className]) != null)
					{
						baseNames.Add(className);						
						className = classNode.BaseName;
					}
				}
				return baseNames; 
			}
		}

		private bool ReferencedNameIsAssignable(string referencedName)
		{
			ArrayList bNames = this.BaseNames; // Use the Property!
			foreach(string s in bNames)
			{
				if (s == referencedName)
					return true;
			}
			return false;
		}

		private bool GegenRelationIs1To1(string ownClassName, ClassNode refClassNode, Relation r)
		{
			// Durchsuche alle Relationen des referenzierten Typs,
			// ob eine davon unseren Typ referenziert und den gleichen Namen hat.
			// Wenn eine gefunden wurde, ermittle, ob sie 
			// ein Listen- oder ein Elementtyp ist.
			
			Class cl = this.mappings.FindClass(refClassNode.Name);
			if (cl == null)
				throw new InternalException(400, "SchemaGenerator: GegenRelationIs1To1");
			foreach (Relation r2 in cl.Relations)
			{
				if (r2.ReferencedTypeName == ownClassName && 
					r2.RelationName == r.RelationName)
				{
					return r2.Multiplicity == RelationMultiplicity.Element;
				}
			}
			return false;
		}



	}
}
#endif