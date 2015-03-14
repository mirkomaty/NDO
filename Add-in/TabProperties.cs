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


using System;
using EnvDTE;

namespace NDOAddIn
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
