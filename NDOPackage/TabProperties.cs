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
using EnvDTE;

namespace NETDataObjects.NDOVSPackage
{
	internal class TabProperty
	{
		short indentSize;
		public short IndentSize
		{
			get { return indentSize; }
		}
		bool useSpaces;
		public bool UseSpaces
		{
			get { return useSpaces; }
		}
		public string Indent
		{
			get 
			{
				if (!useSpaces)
				{
					return "\t";
				}
				else
				{
					string spc = string.Empty;
					for(int count = 0; count < this.indentSize; count++)
						spc += " ";
					return spc;
				}

			}
		}

		public TabProperty(Properties props)
		{
			string s = string.Empty;
			useSpaces = !((bool)props.Item("InsertTabs").Value);
			indentSize = (short) props.Item("IndentSize").Value;
		}
	}

	/// <summary>
	/// Zusammenfassung für TabProperties.
	/// </summary>
	internal class TabProperties
	{
//		static TabProperties instance = new TabProperties();

		public static TabProperties Instance
		{
			get { return new TabProperties(); }
		}

		TabProperty cSharp;
		public TabProperty CSharp
		{
			get { return cSharp; }
		}
		
		TabProperty vBasic;
		public TabProperty VBasic
		{
			get { return vBasic; }
		}
		
		TabProperties()
		{
			try
			{
				_DTE app = ApplicationObject.VisualStudioApplication;
				Properties props = app.get_Properties("TextEditor", "Basic");
				vBasic = new TabProperty(props);
				props = app.get_Properties("TextEditor", "CSharp");
				cSharp = new TabProperty(props);
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.ToString());
			}
		}
	}

	
}
