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

namespace TestGenerator
{
	class Class1
	{
		[STAThread]
		static void Main(string[] args)
		{
			ArrayList relInfos = new ArrayList();
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
			ArrayList newInfos = new ArrayList();
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
