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
		[STAThread]
		static void Main(string[] args)
		{
			List<RelInfo> relInfos = new List<RelInfo>();
			for (int i = 0; i < 128; i++)
			{
                //   isBi             !IsBi           !ForeignIsList
				if ((i & 1) != 0 || ((i & 1) == 0 && (i & 4) == 0))
				{
					RelInfo ri = new RelInfo((i & 1) != 0, (i & 2) != 0, (i & 4) != 0, (i & 8) != 0, (i & 16) != 0, (i & 32) != 0, (i & 64) != 0);
					relInfos.Add(ri);
					if (!ri.HasTable)
					{
						ri =     new RelInfo((i & 1) != 0, (i & 2) != 0, (i & 4) != 0, (i & 8) != 0, (i & 16) != 0, (i & 32) != 0, (i & 64) != 0);
						ri.HasTable = true;
						relInfos.Add(ri);
					}

				}
			}
			List<RelInfo> newInfos = new List<RelInfo>();
			for (int i = 0; i < relInfos.Count; i++)
			{
				RelInfo ri = (RelInfo) relInfos[i];
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
