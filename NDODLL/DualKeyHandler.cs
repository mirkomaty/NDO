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


#if nix
using System;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// This class encapsulates often used operations needed by multiple key oids.
	/// </summary>
	internal class MultiKeyHandler
	{
		Class classMapping;
		Relation rel0;
		Relation rel1;
		Class relClass0;
		Class relClass1;
		public MultiKeyHandler(Class classMapping)
		{
			this.classMapping = classMapping;
			rel0 = classMapping.FindRelation(classMapping.Oid.ParentRelation);
			if (rel0 == null)
				throw new NDOException(76, String.Format("Error while loading related objects: Can't find relation mapping for the field {0}.{1}. Check your mapping file.", classMapping.FullName, classMapping.Oid.ParentRelation));
			rel1 = classMapping.FindRelation(classMapping.Oid.ChildRelation);
			if (rel1 == null)
				throw new NDOException(76, String.Format("Error while loading related objects: Can't find relation mapping for the field {0}.{1}. Check your mapping file.", classMapping.FullName, classMapping.Oid.ChildRelation));
			relClass0 = classMapping.Parent.FindClass(rel0.ReferencedTypeName);
			relClass1 = classMapping.Parent.FindClass(rel1.ReferencedTypeName);
		}

		public string ForeignKeyColumnName(int nr)
		{
			if (nr == 0)
				return rel0.ForeignKeyColumnName;
			return rel1.ForeignKeyColumnName;
		}

		public string ForeignKeyTypeColumnName(int nr)
		{
			if (nr == 0)
				return rel0.ForeignKeyTypeColumnName;
			return rel1.ForeignKeyTypeColumnName;
		}

		public Class GetClass(int nr)
		{
			if (nr == 0)
				return relClass0;
			return relClass1;
		}

		public Relation GetRelation(int nr)
		{
			if (nr == 0)
				return rel0;
			return rel1;
		}

	}
}
#endif