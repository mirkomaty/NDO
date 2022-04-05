//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDOVsPackage
{
	internal class ProjectKind
	{
		public const string prjKindCSharpProject = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		public const string prjKindVBProject = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
		public const string prjKindVSAProject = "{13B7A3EE-4614-11D3-9BC7-00C04F79DE25}";
		public const string prjKindSDECSharpProject = "{20D4826A-C6FA-45db-90F4-C717570B9F32}";
		public const string prjKindSDEVBProject = "{CB4CE8C6-1BDB-4dc7-A4D3-65A1999772F8}";
		public const string prjKindVJSharpProject = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";
		public const string prjKindNewCSharpProject = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";  // This will no longer exist after VS 17.8
	}
}
