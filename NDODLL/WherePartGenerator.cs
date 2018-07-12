#if nix
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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO
{
	/// <summary>
	/// Zusammenfassung für WherePartGenerator.
	/// </summary>
	internal class WherePartGenerator
	{
		IProvider provider;
		string wildcard;
		NDO.Mapping.Class resultClass;
		IList names;
		Mappings mappings;
		IList tokens;
		Hashtable queryContext;
		TypeManager typeManager;

		public override string ToString()
		{
			return GetWherePart();
		}

		public WherePartGenerator(IProvider provider,
			Class resultClass, IList names, IList tokens, Mappings mappings, Hashtable queryContext, TypeManager typeManager)
		{
			this.provider = provider;
			this.wildcard = provider.Wildcard;
			this.resultClass = resultClass;
			this.names = names;
			this.mappings = mappings;
			this.tokens = tokens;
			this.queryContext = queryContext;
			this.typeManager = typeManager;
		}
		/// <summary>
		/// Get the where clause of the SQL string
		/// </summary>
		/// <returns>Where clause string</returns>
		private string GetWherePart()
		{
			if (names.Count == 0) 
				return string.Empty;

			Hashtable columnNames = new Hashtable();
			Dictionary<string, string> tcParameters = new Dictionary<string,string>();
			int tcParameterIndex = 0;

			// Join - Part
			string join = string.Empty;
			// Tabelle die sicherstellt, dass eine Relation nicht 
			// zweimal berücksichtigt wird, zum Beispiel in AND-Konstrukten
			Hashtable relClasses = new Hashtable();
			foreach (string name in names)
			{
				if (name.IndexOf(".") > -1)
				{
					string[] namearr = name.Split(new char[]{'.'});
					Class parentClass = resultClass;
					string valueTypePrefix = null; // != null wenn im vorigen
					// Durchlauf ein VT. gefunden wurde

					for (int i = 0; i < namearr.Length - 1; i++)
					{
						string fieldName = namearr[i];
						Relation rel = parentClass.FindRelation(fieldName);
						if (null == rel)
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
								throw new NDOException(96, "No mapping information found for relation " + parentClass.FullName + "." + name);
							// Es liegt ein Value Type vor. Dies muss der vorletzte Eintrag im namearr sein
							if (i != namearr.Length - 2)
								throw new QueryException(10005, "Wrong ValueType or embedded type member name: " + parentClass.FullName + "." + name);
							valueTypePrefix = vtNameBegin;
							break;
						}
						Class relClass;
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

						if (!relClasses.Contains(relClass))
						{
							relClasses.Add(relClass, string.Empty);
							if (relClass.ConnectionId != parentClass.ConnectionId)
								throw new QueryException(10004, "Can't construct queries across different DB connections");

							if (rel.MappingTable != null)
							{
                                //TODO: Check, if relations to dependent classes are possible
                                if (parentClass.Oid.IsDependent)
                                    throw new NDOException(36, String.Format("Wrong Relation '{0}.{1}'. Relations to intermediate classes are not allowed.", parentClass.FullName, rel.FieldName));
								if (rel.MappingTable.ConnectionId != parentClass.ConnectionId)
									throw new QueryException(10004, "Can't construct queries across different DB connections");

								if (join != string.Empty)
									join += " AND ";
								// OwnClassTable.id = MappingTable.IDOwnClass
                                System.Diagnostics.Debug.Assert(rel.ForeignKeyColumns.Count() == parentClass.Oid.OidColumns.Count);
                                int fkix = 0;
                                foreach (ForeignKeyColumn fkColumn in rel.ForeignKeyColumns)
                                {                                    
                                    OidColumn oidColumn = (OidColumn)parentClass.Oid.OidColumns[fkix];
                                    join += QualifiedTableName.Get(rel.MappingTable.TableName, provider) + "." + provider.GetQuotedName(fkColumn.Name) + " = "
                                        + QualifiedTableName.Get(parentClass.TableName, provider) + "." + provider.GetQuotedName(oidColumn.Name);
                                    if (fkix < rel.ForeignKeyColumns.Count() - 1)
                                        join += " AND ";
                                    fkix++;
                                }
								join += " AND ";
#if PRO

								// MappingTable.TCOwnClass = x
								if (rel.ForeignKeyTypeColumnName != null)
								{
									if (parentClass == resultClass)
									{
										string tcCode = provider.GetSqlLiteral(this.typeManager[resultClass.SystemType]);
										join += QualifiedTableName.Get(rel.MappingTable.TableName, provider) + "." + provider.GetQuotedName(rel.ForeignKeyTypeColumnName) + " = " 
											+ tcCode;
									}
									else
									{
											throw new QueryException(10006, "Can't construct SQL condition string due to the polymorphic type " + parentClass.FullName);
									}
									join += " AND ";
								}
#endif
                                System.Diagnostics.Debug.Assert(rel.MappingTable.ChildForeignKeyColumns.Count() == relClass.Oid.OidColumns.Count);
								// OtherClass.id = MappingTable.IDOtherClass
                                int colIndex = 0;
                                foreach (ForeignKeyColumn fkColumn in rel.MappingTable.ChildForeignKeyColumns)
                                {
                                    OidColumn oidColumn = (OidColumn)relClass.Oid.OidColumns[colIndex];
                                    join += QualifiedTableName.Get(rel.MappingTable.TableName, provider) + "." + provider.GetQuotedName(fkColumn.Name) + " = "
                                        + QualifiedTableName.Get(relClass.TableName, provider) + "." + provider.GetQuotedName(oidColumn.Name);
                                    if (colIndex < relClass.Oid.OidColumns.Count - 1)
                                        join += " AND ";
                                    colIndex++;
                                }
#if PRO
								// MappingTable.TCOtherClass = x
								if (rel.MappingTable.ChildForeignKeyTypeColumnName != null)
								{
									string tcCode = provider.GetSqlLiteral( this.typeManager[relClass.SystemType] );

									if ( string.Compare( namearr[i + 1], "oid", true ) == 0 )
									{
										if ( tcParameters.ContainsKey( name ) )
											tcCode = tcParameters[name];
										else
										{
											tcCode = "{tc:" + tcParameterIndex + "}";
											tcParameters.Add( name, tcCode );
										}
										tcParameterIndex++;
									} 
									join += " AND ";
									join += QualifiedTableName.Get(rel.MappingTable.TableName, provider) + "." + provider.GetQuotedName(rel.MappingTable.ChildForeignKeyTypeColumnName) + " = " 
										+ tcCode;
								}
#endif
							}
							else
							{
								if (join != string.Empty)
									join += " AND ";

								if (rel.Multiplicity != RelationMultiplicity.Element)//(rel.Type == "1:n")
								{
									if (parentClass.Oid.IsDependent)
										throw new NDOException(36, String.Format("Wrong Relation '{0}.{1}'. Relations to intermediate classes are not allowed.", parentClass.FullName, rel.FieldName));
                                    Debug.Assert(rel.ForeignKeyColumns.Count() == parentClass.Oid.OidColumns.Count);

                                    ForeignKeyIterator fkiter = new ForeignKeyIterator(rel);
                                    IEnumerator<OidColumn> oidEnumerator = parentClass.Oid.OidColumns.GetEnumerator();
                                    fkiter.Iterate((fkColumn, isLastEntry) =>
                                    {
                                        oidEnumerator.MoveNext();
                                        OidColumn oidColumn = oidEnumerator.Current;
                                        join += QualifiedTableName.Get(relClass.TableName, provider) + "." + provider.GetQuotedName(fkColumn.Name) + " = "
                                            + QualifiedTableName.Get(parentClass.TableName, provider) + "." + provider.GetQuotedName(oidColumn.Name);
                                        if (!isLastEntry)
                                            join += " AND ";
                                    });
								}
								else // 1:1
								{
									if (relClass.Oid.IsDependent)
										throw new NDOException(36, String.Format("Wrong Relation '{0}.{1}'. Relations to intermediate classes are not allowed.", relClass.FullName, rel.FieldName));
                                    Debug.Assert(rel.ForeignKeyColumns.Count() == relClass.Oid.OidColumns.Count);

                                    ForeignKeyIterator fkiter = new ForeignKeyIterator(rel);
                                    IEnumerator<OidColumn> oidEnumerator = relClass.Oid.OidColumns.GetEnumerator();
                                    fkiter.Iterate((fkColumn, isLastEntry) =>
                                    {
                                        oidEnumerator.MoveNext();
                                        OidColumn oidColumn = oidEnumerator.Current;
                                        join += QualifiedTableName.Get(parentClass.TableName, provider) + "." + provider.GetQuotedName(fkColumn.Name) + " = "
                                            + QualifiedTableName.Get(relClass.TableName, provider) + "." + provider.GetQuotedName(oidColumn.Name);
                                        if (!isLastEntry)
                                            join += " AND ";
                                    });


									if ( rel.ForeignKeyTypeColumnName != null )
									{
										string tcCode = provider.GetSqlLiteral( this.typeManager[relClass.SystemType] );

										if ( string.Compare( namearr[i + 1], "oid", true ) == 0 )
										{
											if ( tcParameters.ContainsKey( name ) )
											{
												tcCode = tcParameters[name];
											}
											else
											{
												tcCode = "{tc:" + tcParameterIndex + "}";
												tcParameters.Add( name, tcCode );
											}
											tcParameterIndex++;
										}
										join += " AND ";
										join += QualifiedTableName.Get( parentClass.TableName, provider ) + "." + provider.GetQuotedName( rel.ForeignKeyTypeColumnName ) + " = "
											+ tcCode;
									}
                                }
							}
						}
						// parentClass will be overwritten until we are in the innermost class
						parentClass = relClass;
					}
					string lastName = namearr[namearr.Length - 1];
					if (valueTypePrefix != null)
					{
						lastName = valueTypePrefix + lastName;
					}

					if (lastName.ToUpper().StartsWith("OID"))
					{
						if (parentClass.Oid == null)
							throw new NDOException(98, "Can't find Oid mapping information for class " + parentClass.FullName);

                        Regex regex = new Regex(@"\(\s*(\d+)\s*\)");
                        Match match = regex.Match(lastName);
                        int index = 0;
                        if (match.Success)
                            index = int.Parse(match.Groups[1].Value);

                        else if (!columnNames.Contains(name))
                        {
                            OidColumn oidColumn = (OidColumn)parentClass.Oid.OidColumns[index];
                            columnNames.Add(name, QualifiedTableName.Get(parentClass.TableName, provider) + "." + provider.GetQuotedName(oidColumn.Name));
                        }						
					}
					else
					{
						Field innerField = parentClass.FindField(lastName);
						if (null == innerField)
							throw new NDOException(7, "Can't find mapping information for field " + parentClass.FullName + "." + namearr[namearr.Length - 1]);
						if (!columnNames.Contains(name))
							columnNames.Add(name, QualifiedTableName.Get(parentClass.TableName, provider) + "." + provider.GetQuotedName(innerField.Column.Name));
					}
				}
				else
				{
					if (name.ToUpper().StartsWith("OID"))
					{
                        Regex regex = new Regex(@"\(\s*(\d+)\s*\)");
                        Match match = regex.Match(name);
                        int index = 0;
                        if (match.Success)
                            index = int.Parse(match.Groups[1].Value);
						if (resultClass.Oid == null)
							throw new NDOException(100, "Can't find Oid mapping information for class " + resultClass.FullName);
						if (resultClass.Oid.IsDependent)
							columnNames.Add(name, "Wrong_Query__Intermediate_Classes_dont_have_oids");
							//As long as the WherePartGenerator is used for Prefetches,
							//we don't throw an exception at this point
							//throw new NDOException(35, "Class '" + resultClass.FullName + "': 'oid' is not a valid condition field for intermediate classes, because the oid is not mapped to a single column.");
                        else if (!columnNames.Contains(name))
                        {
                            if (resultClass.Oid.OidColumns.Count < index + 1)
                                throw new NDOException(114, "Wrong oid index in query: '" + name + "'. Size of Oid column array of type " + resultClass.FullName + " is " + resultClass.Oid.OidColumns.Count + '.');
							if ( match.Success )
							{
								OidColumn oidColumn = (OidColumn) resultClass.Oid.OidColumns[index];
								columnNames.Add( name, QualifiedTableName.Get( resultClass.TableName, provider ) + "." + provider.GetQuotedName( oidColumn.Name ) );
							}
                        }
						
					}
					else
					{
						Column column = null;
						Field f = resultClass.FindField(name);
						if (null != f)
							column = f.Column;
						else
						{
							Relation r = resultClass.FindRelation( name );
							if (r.Multiplicity == RelationMultiplicity.Element && r.ForeignKeyColumns.Count() == 1)
								column = r.ForeignKeyColumns.First();
						}
						if (column == null)
							throw new NDOException(7, "Can't find mapping information for field " + resultClass.FullName + "." + name);
						if (!columnNames.Contains(name))
							columnNames.Add(name, QualifiedTableName.Get(resultClass.TableName, provider) + "." + provider.GetQuotedName(column.Name));
					}
				}
			}

			// where bzw. Filter-Part

			string newfilter = "";
			string lastUsedName = null;
			Token lastOperator = null;

			foreach (Token t in tokens)
			{
				if (t.TokenType == Token.Type.Name)
				{
					if ( string.Compare( lastUsedName, "oid", true ) == 0 )
					{
						newfilter += "oid";
					}
					newfilter += columnNames[t.Value];
					lastUsedName = (string) t.Value;
				}
				else if ( t.IsOperator )
				{
					if ( string.Compare( lastUsedName, "oid", true ) == 0 )
						lastOperator = t;
					else
						newfilter += t.Value;
				}
				else if ( t.TokenType == Token.Type.Parameter )
				{
					int parIndex = ( (ScParameter) t ).Index;
					if (lastUsedName != null && tcParameters.ContainsKey( lastUsedName ) )
					{
						string parStr = tcParameters[lastUsedName];
						string newParStr = "{tc:" + parIndex + "}";
						if ( parStr != newParStr )
							join = join.Replace( parStr, newParStr );
					}
					if ( string.Compare( lastUsedName, "oid", true ) == 0 )
					{
						int i = 0;
						int loopEnd = resultClass.Oid.OidColumns.Count - 1;
						foreach ( OidColumn oidColumn in resultClass.Oid.OidColumns )
						{
							string col = QualifiedTableName.Get( resultClass.TableName, provider ) + "." + provider.GetQuotedName( oidColumn.Name );
							newfilter += col;
							newfilter += " ";
							newfilter += lastOperator.Value;
							newfilter += " ";
							newfilter += "{oid:" + parIndex + ":" + i + "}";
							if ( i < loopEnd )
								newfilter += " AND ";
							i++;
						}
					}
					else
					{
						newfilter += t.Value;
					}
					lastUsedName = null;
				}
				else if ( t.TokenType == Token.Type.StringLiteral )
				{
					string lit = (string) t.Value;
					if ( wildcard != "%" )
						lit = lit.Replace( "%", wildcard );
					if ( wildcard != "*" )
						lit = lit.Replace( "*", wildcard );
					newfilter += lit;
				}
				else if ( t.TokenType == Token.Type.FileTime )
				{
					DateTime dt;
					dt = DateTime.FromFileTime( (long) t.Value );
					newfilter += provider.GetSqlLiteral( dt );
				}
				else if ( t.TokenType == Token.Type.FileTimeUtc )
				{
					DateTime dt;
					dt = DateTime.FromFileTimeUtc( (long) t.Value );
					newfilter += provider.GetSqlLiteral( dt );
				}
				else
					newfilter += t.Value.ToString();
				newfilter += " ";
			}

			string where = "WHERE ";

			if (join != string.Empty)
				where += "(" + join + ")";
			if (newfilter.Trim() != string.Empty)
			{
				if (join != string.Empty)
					where += " AND (";
				where += newfilter;
                if (join != string.Empty)
                    where += ")";
            }

			return where;
		}

	}
}

#endif