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
