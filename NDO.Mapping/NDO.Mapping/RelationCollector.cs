//
// Copyright (c) 2002-2024 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using System.Collections;
using System.Data;
using NDO;
using NDO.Mapping;
using NDOInterfaces;
using System.Collections.Generic;

namespace NDO.Mapping
{
	/// <summary>
	/// Contains algorithms to determine the relations, which have a foreign key
	/// column in the table of a given class.
	/// </summary>
	internal class RelationCollector
	{
		Class classMapping;
		NDOMapping mappings;
		IProvider provider;
		List<string> fkColums;
        Hashtable collectedRelations;

		public IEnumerable<string> ForeignKeyColumns
		{
			get { return fkColums; }
		}

		public RelationCollector(Class classMapping)
		{
			this.classMapping = classMapping;
			mappings = classMapping.Parent;
			this.provider = classMapping.Provider;
		}

        void AddInfos(Relation r, List<RelationFieldInfo> relationInfos)
        {
            foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
            {
                if (!fkColums.Contains(fkColumn.Name))
                {
                    fkColums.Add(fkColumn.Name);
                }
            }
            if (r.ForeignKeyTypeColumnName != null
                && !fkColums.Contains(r.ForeignKeyTypeColumnName))
                fkColums.Add(r.ForeignKeyTypeColumnName);
            if (!collectedRelations.Contains(r))
            {
                collectedRelations.Add(r, null);
                relationInfos.Add(new RelationFieldInfo(r, classMapping.TableName));
            }
        }


		public IEnumerable<RelationFieldInfo> CollectRelations()
		{
            this.collectedRelations = new Hashtable();
			List<RelationFieldInfo> relationInfos = new List<RelationFieldInfo>();
			this.fkColums = new List<string>();

			// Collect all element relations of the own class
			foreach (Relation r in classMapping.Relations)
			{
                // Search for all relations, where the own table contains the foreign key
				if (r.Multiplicity == RelationMultiplicity.Element && r.MappingTable == null) 
				{
                    AddInfos(r, relationInfos);
				}
			}

			// collect all list relations directed to the own class
			foreach (NDO.Mapping.Class c in mappings.Classes)
			{
				foreach (Relation r in c.Relations)
				{
					if (r.MappingTable != null)
						continue;
					if (r.Multiplicity == RelationMultiplicity.Element)
						continue;
					// List relation to the given class - we must hold the foreign key
					if (r.ReferencedTypeName == classMapping.FullName)
					{
                        AddInfos(r, relationInfos);
					}
				}
			}

			return relationInfos;
		}

	}
}
