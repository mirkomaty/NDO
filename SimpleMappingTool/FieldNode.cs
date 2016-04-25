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
using System.Windows.Forms;
using NDO.Mapping;

namespace SimpleMappingTool
{
	/// <summary>
	/// Zusammenfassung für FieldNode.
	/// </summary>
	internal class FieldNode : NDOTreeNode
	{
		public FieldNode(Field field) : base (ConvertFieldName(field.Name), field)
		{
			this.ImageIndex = 2;
			this.SelectedImageIndex = 2;
            this.Nodes.Add(new ColumnNode(this, field.Column));
		}

		static string ConvertFieldName(string s)
		{
			//Automatic fields have names like that: <Position>k__BackingField
			if (s[0]=='<')
			{
				int p = s.IndexOf( '>' );
				if (p > -1)
					return s.Substring( 1, p - 1 );
			}
			return s;
		}
	}
}
