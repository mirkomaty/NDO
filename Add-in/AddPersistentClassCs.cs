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
using System.Text;
using System.IO;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif

namespace NDOAddIn
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
			StreamWriter sw = null;

			try
			{
				string fileName = Path.GetDirectoryName(project.FileName) + "\\" + className + ".cs";
				string partialFileName = fileName.Substring( 0, fileName.Length - 3 );
				partialFileName += ".ndo.cs";
				sw = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);

				StringBuilder sb = new StringBuilder();
                
				sb.Append("using System;\n");
				sb.Append("using System.Collections;\n");
#if !NDO11
                sb.Append("using System.Collections.Generic;\n");
#endif
				sb.Append("using NDO;\n\n");
				string namespc = (string) project.Properties.Item("RootNamespace").Value;
				sb.Append("namespace " + namespc + "\n");
				sb.Append("{\n");

				sb.Append("\t/// <summary>\n");
				sb.Append("\t/// Summary for " + className + "\n");
				sb.Append("\t/// </summary>\n");
				sb.Append("\t[NDOPersistent");
                if (this.isSerializable)
                    sb.Append(", Serializable");
                sb.Append("]\n");
                sb.Append("\tpublic partial class " + className + "\n");
				sb.Append("\t{\n");
				sb.Append("\t\tpublic " + className + "()\n");
				sb.Append("\t\t{\n");
				sb.Append("\t\t}\n");
				sb.Append("\t}\n");
				sb.Append("}\n");			
				string result = sb.ToString();
				TabProperty tp = TabProperties.Instance.CSharp;
				if (tp.UseSpaces)
					sw.Write(result.Replace("\t", tp.Indent));
				else
					sw.Write(result);
				sw.Close();
				ProjectItem pi = null;
				if ( parentItem == null )
					pi = project.ProjectItems.AddFromFile( fileName );
				else
					pi = parentItem.ProjectItems.AddFromFile( fileName );

				sw = new StreamWriter( partialFileName );
				string newPartial = partialTemplate.Replace( "#ns#", namespc );
				newPartial = newPartial.Replace( "#cl#", className );
				sw.Write( newPartial );
				sw.Close();
				pi.ProjectItems.AddFromFile( partialFileName );
				CodeGenHelper.ActivateAndGetTextDocument(project, Path.GetFileName(fileName));
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (sw != null)
					sw.Close();
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
