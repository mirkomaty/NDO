//
// Copyright (c) 2002-2016 Mirko Matytschak 
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

namespace TestGenerator
{
	/// <summary>
	/// Summary for RelInfo.
	/// </summary>
	public class RelInfo
	{
		bool isBi;
		public bool IsBi
		{
			get { return isBi; }
			set { isBi = value; }
		}
		bool isList;
		public bool IsList
		{
			get { return isList; }
			set { isList = value; }
		}
		bool foreignIsList;
		public bool ForeignIsList
		{
			get { return foreignIsList; }
			set { foreignIsList = value; }
		}
		bool isComposite;
		public bool IsComposite
		{
			get { return isComposite; }
			set { isComposite = value; }
		}
		bool hasTable;
		public bool HasTable
		{
			get { return hasTable; }
			set { hasTable = value; }
		}
		bool ownPoly;
		public bool OwnPoly
		{
			get { return ownPoly; }
			set { ownPoly = value; }
		}
		bool otherPoly;
		public bool OtherPoly
		{
			get { return otherPoly; }
			set { otherPoly = value; }
		}
		bool isAbstract;
		public bool IsAbstract
		{
			get { return isAbstract; }
			set { isAbstract = value; }
		}
		bool useGuid;
		public bool UseGuid
		{
			get { return useGuid; }
			set { useGuid = value; }
		}

		public RelInfo(bool isbi, bool islist, bool fislist, bool iscomp, bool ownpoly, bool othpoly, bool useguid)
		{
			this.isBi = isbi;
			this.isList = islist;
			this.foreignIsList = fislist;
			this.isComposite = iscomp;
			this.ownPoly = ownpoly;
			this.otherPoly = othpoly;
			this.hasTable = MustHaveTable;
			this.useGuid = useguid;
		}

		public bool MustHaveTable
		{
			get
			{
				return
				   (isBi && isList && foreignIsList)					
				|| (otherPoly && isList)					
				|| (isBi && ownPoly && foreignIsList);
			}
		}

		public override string ToString()
		{
			string result = isComposite ? "Cmp" : "Agr";
			if (isBi)
				result += "Bi";
			else
				result += "Dir";
			if (isList)
				result += "n";
			else
				result += "1";
			if (isBi)
			{
				if (foreignIsList)
					result += "n";
				else
					result += "1";
			}
			if (ownPoly)
			{
				result += "Ownp";
				if (isAbstract)
					result += "abs";
				else
					result += "con";
			}
			if (otherPoly)
			{
				result += "Othp";
				if (isAbstract)
					result += "abs";
				else
					result += "con";
			}
			if (hasTable)
				result += "Tbl";
			else
				result += "NoTbl";
			if (useGuid)
				result += "Guid";
			else
				result += "Auto";
			return result;
		}
		public RelInfo Clone()
		{
			RelInfo ri = new RelInfo(this.isBi, this.isList,this.foreignIsList, this.isComposite, this.ownPoly, this.otherPoly, this.useGuid);
			ri.hasTable = this.hasTable;
			ri.isAbstract = this.isAbstract;
			return ri;
		}

		public RelInfo GetReverse()
		{
			RelInfo ri = new RelInfo(this.isBi, this.foreignIsList, this.IsList, false, this.otherPoly, this.ownPoly, this.useGuid);
			ri.hasTable = this.hasTable;
			ri.isAbstract = this.isAbstract;
			return ri;
		}

	}


}
