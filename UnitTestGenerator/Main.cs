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
using System.Collections;
using System.Collections.Generic;

namespace TestGenerator
{
	class Class1
	{
		static readonly int isBi			=	1;
		static readonly int isList			=	2;
		static readonly int foreignIsList	=	4;
		static readonly int isComposite		=	8;
		static readonly int ownPoly			=	16;
		static readonly int otherPoly		=	32;
		static readonly int useGuid			=	64;


		[STAThread]
		static void Main(string[] args)
		{
			List<RelInfo> relInfos = new List<RelInfo>();
			for (int i = 0; i < 128; i++)
			{
                //   isBi      ||        !IsBi          &&  !ForeignIsList
				if ((i & isBi) != 0 || ((i & isBi) == 0 && (i & foreignIsList) == 0))
				{
					RelInfo ri = new RelInfo((i & isBi) != 0, (i & isList) != 0, (i & foreignIsList) != 0, (i & isComposite) != 0, (i & ownPoly) != 0, (i & otherPoly) != 0, (i & useGuid) != 0);
					relInfos.Add(ri);
					if (!ri.MustHaveTable)
					{
						// We duplicate every scenario without a mapping table
						// and try to map using a mapping table
						ri =     new RelInfo((i & isBi) != 0, (i & isList) != 0, (i & foreignIsList) != 0, (i & isComposite) != 0, (i & ownPoly) != 0, (i & otherPoly) != 0, (i & useGuid) != 0);
						ri.HasTable = true;
						relInfos.Add(ri);
					}
				}
			}

			// We duplicate all polymorphic scenarios
			// and use abstract base classes.
			List<RelInfo> newInfos = new List<RelInfo>();
			for (int i = 0; i < relInfos.Count; i++)
			{
				RelInfo ri = relInfos[i];
				if (ri.OtherPoly || ri.OwnPoly)
				{
					ri = ri.Clone();
					ri.IsAbstract = true;
					newInfos.Add(ri);
				}
			}

			relInfos.AddRange(newInfos);

			new ClassGenerator(relInfos).Generate();
			new TestGenerator(relInfos).Generate();
			Console.WriteLine(relInfos.Count);
//			for (int i = 0; i < relInfos.Count; i++)
//			{
//				Console.WriteLine(relInfos[i].ToString());
//			}
//			Console.WriteLine(relInfos.Count);
		}
	}
}
