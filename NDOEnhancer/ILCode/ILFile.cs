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
using System.IO;
using System.Text;
using System.Collections;

namespace ILCode
{
	/// <summary>
	/// Summary description for ILFile.
	/// </summary>
	internal class ILFile : ILElement
	{
		static ILFile()
		{
			ILBlankElement.initialize();
			ILCommentElement.initialize();
			ILAssemblyElement.initialize();
			ILCustomElement.initialize();
			ILModuleElement.initialize();
			ILPublickeytokenElement.initialize();
			ILPublickeyElement.initialize();
			ILNamespaceElement.initialize();
			ILClassElement.initialize();
			ILFieldElement.initialize();
			ILPropertyElement.initialize();
			ILGetElement.initialize();
			ILSetElement.initialize();
			ILMethodElement.initialize();
			ILTryElement.initialize();
			ILCatchElement.initialize();
			ILStatementElement.initialize();
			ILLineElement.initialize();
			ILLocalsElement.initialize();
			ILMaxstackElement.initialize();
			
			ILUnknownElement.initialize();
		}

		public ILFile()
			: base( "", null )
		{
		}

		StreamReader				m_streamIn;
		StreamWriter				m_streamOut;
		string						m_lineBuffer = "";
		string						m_filename;

		public void
		parse( string filename )
		{
			m_filename = filename;
			m_streamIn = new StreamReader( filename, Encoding.UTF8 );

			parseSubElements( this );

			m_streamIn.Close();
			m_streamIn = null;

			ILAssemblyElement.Iterator assIter = getAssemblyIterator();
			for ( ILAssemblyElement assElem = assIter.getNext(); null != assElem; assElem = assIter.getNext() )
			{
				if ( ! assElem.isExtern() )
				{
					setAssemblyName( assElem.getName() );
					break;
				}
			}
		}

		public void
		writeWithPreExtension( string preExtension )
		{
			write( Path.GetDirectoryName( m_filename )
					+ Path.DirectorySeparatorChar
					+ Path.GetFileNameWithoutExtension( m_filename )
					+ "."
					+ preExtension + Path.GetExtension( m_filename ) );
		}

		public string
		getFileName()
		{
			return m_filename;
		}

		public void
		write( string file )
		{
			m_streamOut = new StreamWriter( file, false, Encoding.UTF8);

			writeSubElements( this, 0 );

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
					line = stripComment( line );
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
		removeAssemblyReference( string assemblyName )
		{
			ILAssemblyElement.Iterator assIter = getAssemblyIterator();

			for ( ILAssemblyElement assElem = assIter.getFirst(); null != assElem; assElem = assIter.getNext() )
			{
				if ( assElem.isExtern() && assElem.getName() == assemblyName )
					assElem.remove();
			}

			base.removeAssemblyReference( assemblyName );
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


	
	}
}
