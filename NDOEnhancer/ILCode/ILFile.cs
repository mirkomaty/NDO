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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ILCode
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

		StreamReader				m_streamIn;
		StreamWriter				m_streamOut;
		string						m_lineBuffer = "";
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
				throw new Exception( "ILFile doesn't have an assembly extern element" );

			SetAssemblyName( externElement.Name );
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
	
		public string
		popLine()
		{
			string line;

			if ( 0 < m_lineBuffer.Length )
			{
				line = m_lineBuffer;
				m_lineBuffer = "";
			}
			else
			{
				line = m_streamIn.ReadLine();
				if ( null != line )
				{
					line = StripComment( line );
				}
			}

			return line;

		}

		public void
		pushLine( string line )
		{
			m_lineBuffer = line;
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

		public bool
		testPersistence( string file )
		{
			string info = "";
			try
			{
				if (file == null)
					info = "file: null ";
				else
					info += "file: " + file + " ";

				m_streamIn = new StreamReader( file, Encoding.UTF8 );

				if (m_streamIn == null)
					info += "m_streamIn: null ";
				else
					info += "m_streamIn: non null ";

				for ( string line = popLine(); null != line ; line = popLine() )
				{
					if ( line.IndexOf( "[NDO]NDO.IMetaClass" ) > -1)
						return true;
				}

				return false;
			}
			catch (Exception ex)
			{
				string message = ex.Message + "\n" + info;
				throw new Exception(message);
			}
			finally
			{
				if (null != m_streamIn)
				{
					m_streamIn.Close();
					m_streamIn = null;
				}
			}
		}

		public IEnumerable<ILAssemblyElement> AssemblyElements => m_assemblyElements;
	
	}
}
