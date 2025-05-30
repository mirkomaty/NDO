//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILFile.
	/// </summary>
	internal class ILFile : ILElement
	{
		public ILFile()
			: base( "", null )
		{
		}

		/// <summary>
		/// Static constructor. Registers all creatable element types.
		/// </summary>
		static ILFile()
		{
			var assembly = typeof(ILFile).Assembly;
			var types = assembly.GetTypes()
				.Where( t => typeof(ILElement).IsAssignableFrom(t) );

			foreach(Type t in types)
			{
				var field = t.GetField( "m_elementType", BindingFlags.NonPublic | BindingFlags.Static );
				if (field != null && field.FieldType == typeof(ILElementType))
				{
					var elementType = (ILElementType)field.GetValue(null);
					if (elementType != null)
						ILElement.AddElementType( elementType );
				}
			}
		}

		StreamReader				m_streamIn;
		StreamWriter				m_streamOut;
		string						m_lookAheadLine = String.Empty;
		string						m_filename;
		List<ILAssemblyElement>		m_assemblyElements = null;

		public void
		Parse( string filename )
		{
			m_filename = filename;
			m_streamIn = new StreamReader( filename, Encoding.UTF8 );

			ParseSubElements( this );

			m_streamIn.Close();
			m_streamIn = null;

			m_assemblyElements = ( from e in Elements
								   let ae = e as ILAssemblyElement
								   where ae != null select ae).ToList();

			var externElement = (from ae in m_assemblyElements where !ae.IsExtern select ae).FirstOrDefault();
			if (externElement == null)
				throw new Exception( "ILFile doesn't have an assembly element" );

			AssemblyName = externElement.Name;
		}

		public string
		getFileName()
		{
			return m_filename;
		}

		public void
		write( string file, bool isNetStandard )
		{
			m_streamOut = new StreamWriter( file, false, Encoding.UTF8);

			WriteSubElements( this, 0, isNetStandard );

			m_streamOut.Close();
			m_streamOut = null;
		}
	
		public string PopLine()
		{
			string line;

			if ( m_lookAheadLine != String.Empty )
			{
				line = m_lookAheadLine;
				m_lookAheadLine = String.Empty;
				return line;
			}

			do
			{
				line = m_streamIn.ReadLine();
				if (null != line)
				{
					line = StripComment( line );
				}
			} while (line != null && line == String.Empty);

			return line;

		}

		public void PushLine( string line )
		{
			m_lookAheadLine = line;
		}

		public void
		writeLine( int level, string line )
		{
			m_streamOut.WriteLine( new string( ' ', 2 * level ) + line );
		}

		public override void
		RemoveAssemblyReference( string assemblyName )
		{
			var externElement = (from ae in m_assemblyElements 
								 where ae.IsExtern && ae.Name == assemblyName 
								 select ae).FirstOrDefault();

			if (externElement == null)
				return;

			externElement.Remove();
		}

		public IEnumerable<ILAssemblyElement> AssemblyElements => m_assemblyElements;
	
	}
}
