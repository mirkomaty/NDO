//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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

namespace Cli
{
	/// <summary>
	/// Zusammenfassung für Class1.
	/// </summary>
	internal class PeTool
	{
#if DEBUG
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (PeTool.IsAssembly(@"C:\Programme\text & talk GmbH\NDO Enterprise Version\ndo.dll"))
				Console.WriteLine("Ist CLI");
			else
				Console.WriteLine("Keine CLI");
			
			if (PeTool.IsAssembly(@"C:\Programme\text & talk GmbH\NDO Enterprise Version\ndoenhancer.dll"))
				Console.WriteLine("Ist CLI");
			else
				Console.WriteLine("Keine CLI");

			Console.ReadLine();
		}
#endif
		public static bool IsAssembly(string peFile)
		{
			FileInfo pe = new FileInfo(peFile);
			if (!pe.Exists) 
			{
				throw new Exception("Can't find File " + peFile);
			}

			using (BinaryReader reader = new BinaryReader(pe.OpenRead())) 
			{
				DOSHeader dosHdr = new DOSHeader();
				COFFHeader coffHdr = new COFFHeader();
				PEHeader peHdr = new PEHeader();

				dosHdr.Read (reader);
				reader.BaseStream.Position = dosHdr.Lfanew;

				ushort peSig = reader.ReadUInt16();
				if (peSig != 0x4550) 
					return false;
	
				peSig = reader.ReadUInt16();
				if (peSig != 0) 
					return false;

				coffHdr.Read(reader);
				peHdr.Read(reader);

				return peHdr.IsCLIImage;

			}
		}
	}
}
