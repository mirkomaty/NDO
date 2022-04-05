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
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace NDOVsPackage.Commands
{
	/// <summary>
	/// Zusammenfassung für AddPersistentClassVb.
	/// </summary>
	internal class AddPersistentClassVb
	{
		Project project;
		string className;
        bool isSerializable;
		ProjectItem parentItem;

		public AddPersistentClassVb(Project project, string className, bool isSerializable, ProjectItem parentItem)
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
				string fileName = Path.GetDirectoryName( project.FileName ) + "\\" + className + ".vb";
				string partialFileName = fileName.Substring( 0, fileName.Length - 3 );
				partialFileName += ".ndo.vb";
				using (sw = new StreamWriter( fileName, false, System.Text.Encoding.UTF8 ))
				{

					sw.WriteLine( "Imports System.Linq" );
					sw.WriteLine( "Imports System.Collections.Generic" );
					sw.WriteLine( "Imports NDO\n" );

					sw.WriteLine( "''' <summary>" );
					sw.WriteLine( "'''" );
					sw.WriteLine( "''' </summary>" );
					sw.WriteLine( "''' <remarks></remarks>" );
					sw.Write( "<NDOPersistent" );
					if (isSerializable)
						sw.Write( ", Serializable" );
					sw.WriteLine( "> _" );
					sw.WriteLine( "Partial Public Class " + className );
					sw.WriteLine( "End Class" );
				}
				ProjectItem pi = null;
				if ( parentItem == null )
					pi = project.ProjectItems.AddFromFile( fileName );
				else
					pi = parentItem.ProjectItems.AddFromFile( fileName );
				using (sw = new StreamWriter( partialFileName ))
				{
					string newPartial = partialTemplate.Replace( "#cl#", className );
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

		readonly string partialTemplate = @"Imports NDO
' Don't change this code.
' This interface implementation exists only for intellisense support.
' Any code in this file will be replaced by the enhancer.
Partial Public Class #cl#
    Implements IPersistentObject

    Public Sub NDOMarkDirty() Implements NDO.IPersistentObject.NDOMarkDirty

    End Sub

    Public Property NDOObjectId() As NDO.ObjectId Implements NDO.IPersistentObject.NDOObjectId
        Get
            Throw New NotImplementedException
        End Get
        Set(ByVal value As NDO.ObjectId)
            Throw New NotImplementedException
        End Set
    End Property

    Public Property NDOObjectState() As NDO.NDOObjectState Implements NDO.IPersistentObject.NDOObjectState
        Get
            Throw New NotImplementedException
        End Get
        Set(ByVal value As NDO.NDOObjectState)
            Throw New NotImplementedException
        End Set
    End Property

    Public Property NDOTimeStamp() As System.Guid Implements NDO.IPersistentObject.NDOTimeStamp
        Get
            Throw New NotImplementedException
        End Get
        Set(ByVal value As System.Guid)
            Throw New NotImplementedException
        End Set
    End Property
End Class
";
	}
}
