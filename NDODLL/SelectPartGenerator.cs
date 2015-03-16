//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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
using System.Collections;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für SelectPartGenerator.
	/// </summary>
	internal class SelectPartGenerator
	{
		IProvider provider;
		string wildcard;
		NDO.Mapping.Class resultClass;
		IList names;
		Mappings mappings;
		Hashtable queryContext;

		public override string ToString()
		{
			return GetSelectPart();
		}

		public SelectPartGenerator(IProvider provider,
			Class resultClass, IList names, Mappings mappings, Hashtable queryContext)
		{
			this.provider = provider;
			this.wildcard = provider.Wildcard;
			this.resultClass = resultClass;
			this.names = names;
			this.mappings = mappings;
			this.queryContext = queryContext;
		}


		private void CheckForValueType(Class parentClass, string name, string fieldName, bool vorletzter)
		{
			Field fieldInfo = null;
			string vtNameBegin = fieldName + '.';
			foreach(Field f in parentClass.Fields)
			{
				if (f.Name.StartsWith(vtNameBegin))
				{
					fieldInfo = f;
					break;
				}
			}
			if (null == fieldInfo)
				throw new NDOException(8, "Can't find mapping information for relation " + parentClass.FullName + "." + fieldName);
			// Es liegt ein Value Type vor. Dies muss der vorletzte Eintrag im namearr sein
			if (!vorletzter)
				throw new QueryException(10003, "Wrong ValueType or embedded type member name: " + parentClass.FullName + "." + name);
		}


		/// <summary>
		/// Durchlaufe rekursiv die Klassenbeschreibungen bis alle Subklassen dran waren.
		/// </summary>
		/// <param name="parentClass">Elternklasse der Beziehung</param>
		/// <param name="relClass">Kindklasse bzw. eine Subklasse davon</param>
		/// <param name="fromTable">Hashtable zum Durchreichen an AddRelationTable</param>
		/// <returns></returns>
		private string CheckSubClasses(Class parentClass, Class relClass, Hashtable fromTable)
		{
			if (relClass.SystemType.IsAbstract && !relClass.SystemType.IsInterface)
				return string.Empty;
				//throw new QueryxException("Can't determine join table for type " + relClass.FullName);
			return AddRelationTable(parentClass, relClass, fromTable);
#if Alt // 23.02.05
			string from = string.Empty;
			if (!relClass.SystemType.IsAbstract && !relClass.SystemType.IsInterface)
				from += AddRelationTable(parentClass, relClass, fromTable);
			foreach (Class cl in relClass.Subclasses)
				from += CheckSubClasses(parentClass, cl, fromTable);
			return from;
#endif
		}


		/// <summary>
		/// Trage eine Tabelle für eine Class in die From-List ein. 
		/// Vorher gibts noch ein paar Sicherheitschecks.
		/// </summary>
		/// <param name="parentClass">Elternklasse der Beziehung; wird nur für Fehlermeldungen gebraucht.</param>
		/// <param name="relClass">Klasse, deren Tabelle aufgenommen werden soll.</param>
		/// <param name="fromTable">Hashtabelle, in der alle bereits aufgenommenen Tabellen gelistet werden, um Mehrfachnennungen zu vermeiden</param>
		/// <returns></returns>
		private string AddRelationTable(Class parentClass, Class relClass, Hashtable fromTable)
		{
			if (relClass.ConnectionId != parentClass.ConnectionId)
				throw new QueryException(10004, "Can't construct queries across different DB connections");
			if (!fromTable.Contains(relClass.TableName))
			{
				fromTable.Add(relClass.TableName, string.Empty);
				return QualifiedTableName.Get(relClass.TableName, provider) + ", ";	
			}
			return string.Empty;
		}

		/// <summary>
		/// Get the select list to be used in the SQL query
		/// </summary>
		/// <returns>Select list string</returns>
		private string GetSelectPart()
		{
			string from = QualifiedTableName.Get(resultClass.TableName, provider) + ", ";
			Hashtable fromTable = new Hashtable();

			foreach (string name in names)
			{
				// Select-Part ändert sich nur, wenn ein Join vorliegt
				if (name.IndexOf(".") > -1)
				{
					string[] namearr = name.Split(new char[]{'.'});
					Class parentClass = resultClass;
					for (int i = 0; i < namearr.Length - 1; i++)
					{
						string fieldName = namearr[i];
						Relation rel = parentClass.FindRelation(fieldName);
						if (null == rel)
						{
							// Wenns der vorletzte Index ist, dann kann es ein ValueType sein, der verglichen werden soll
							// In dem Fall tun wir nichts, da hier nur Relations interessieren.
							CheckForValueType( parentClass, name, fieldName, i == namearr.Length - 2 );
							break;
						}

						// Wenns eine MappingTable gibt, dann addiere sie zur From-Liste
						if (null != rel.MappingTable)
						{
							if (rel.MappingTable.ConnectionId != parentClass.ConnectionId)
								throw new QueryException(10004, "Can't construct queries across different DB connections");
							if (!fromTable.Contains(rel.MappingTable.TableName))
							{
								from += QualifiedTableName.Get(rel.MappingTable.TableName, provider) + ", ";	
								fromTable.Add(rel.MappingTable.TableName, string.Empty);
							}
						}

						Class relClass;
						// Die bezogene Tabelle hinzufügen
#if PRO
						// In case of polymorphic queries
						relClass = null;
						if (queryContext.Contains(rel))
							relClass = (Class) queryContext[rel];
						else
#endif
							relClass = mappings.FindClass(rel.ReferencedTypeName);

						if (null == relClass)
							throw new NDOException(17, "Can't find mapping information for class " + rel.ReferencedTypeName);
						from += CheckSubClasses(parentClass, relClass, fromTable);

						parentClass = relClass;
					}
				}
			}

			// die letzten zwei Zeichen ", " entfernen
			from = from.Substring(0, from.Length - 2);
			if (fromTable.Count > 0)
				return "SELECT DISTINCT " + Query.FieldMarker + " FROM " + from;
			else
				return "SELECT " + Query.FieldMarker + " FROM " + from;
		}

	}
}
