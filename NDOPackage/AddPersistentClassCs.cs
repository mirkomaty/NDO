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
using System.Text;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace NETDataObjects.NDOVSPackage
{
	/// <summary>
	/// Zusammenfassung für AddPersistentClassCs.
	/// </summary>
	internal class AddPersistentClassCs
	{
		Project project;
		string className;
        bool isSerializable;
		ProjectItem parentItem;

		public AddPersistentClassCs(Project project, string className, bool isSerializable, ProjectItem parentItem)
		{
			this.project = project;
			this.className = className;
            this.isSerializable = isSerializable;
			this.parentItem = parentItem;
		}

		public void DoIt()
		{
			try
			{
				StreamWriter sw;
				string fileName = Path.GetDirectoryName(project.FileName) + "\\" + className + ".cs";
				string partialFileName = fileName.Substring( 0, fileName.Length - 3 );
				partialFileName += ".ndo.cs";
				string namespc = (string) project.Properties.Item( "RootNamespace" ).Value;
				using (sw = new StreamWriter( fileName, false, System.Text.Encoding.UTF8 ))
				{
					StringBuilder sb = new StringBuilder();

					sb.Append( "using System;\n" );
					sb.Append( "using System.Linq;\n" );
					sb.Append( "using System.Collections.Generic;\n" );
					sb.Append( "using NDO;\n\n" );					
					sb.Append( "namespace " + namespc + "\n" );
					sb.Append( "{\n" );

					sb.Append( "\t/// <summary>\n" );
					sb.Append( "\t/// Summary for " + className + "\n" );
					sb.Append( "\t/// </summary>\n" );
					sb.Append( "\t[NDOPersistent" );
					if (this.isSerializable)
						sb.Append( ", Serializable" );
					sb.Append( "]\n" );
					sb.Append( "\tpublic partial class " + className + "\n" );
					sb.Append( "\t{\n" );
					sb.Append( "\t\tpublic " + className + "()\n" );
					sb.Append( "\t\t{\n" );
					sb.Append( "\t\t}\n" );
					sb.Append( "\t}\n" );
					sb.Append( "}\n" );
					string result = sb.ToString();
					TabProperty tp = TabProperties.Instance.CSharp;
					if (tp.UseSpaces)
						sw.Write( result.Replace( "\t", tp.Indent ) );
					else
						sw.Write( result );
				}
				ProjectItem pi = null;
				if ( parentItem == null )
					pi = project.ProjectItems.AddFromFile( fileName );
				else
					pi = parentItem.ProjectItems.AddFromFile( fileName );

				using (sw = new StreamWriter( partialFileName ))
				{
					string newPartial = partialTemplate.Replace( "#ns#", namespc );
					newPartial = newPartial.Replace( "#cl#", className );
					sw.Write( newPartial );
				}

				pi.ProjectItems.AddFromFile( partialFileName );
				CodeGenHelper.ActivateAndGetTextDocument(project, Path.GetFileName(fileName));
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		readonly string partialTemplate = @"using System;
using NDO;

namespace #ns#
{
	// Don't change this code.
	// This interface implementation exists only for intellisense support.
	// Any code in this file will be replaced by the enhancer.
	public partial class #cl# : IPersistentObject
	{
		#region IPersistentObject Members

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}

		public ObjectId NDOObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public NDOObjectState NDOObjectState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Guid NDOTimeStamp
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
";
	}
}
