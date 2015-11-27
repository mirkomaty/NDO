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
using System.Runtime.InteropServices;

namespace Cli {

	// IMAGE_OPTIONAL_HEADER
	internal class PEHeader {
		internal class StdFields {
			internal short magic;
			internal byte lMajor;
			internal byte lMinor;
			internal uint codeSize;
			internal uint initDataSize;
			internal uint uninitDataSize;
			internal RVA  entryRVA;
			internal RVA  codeBase;
			internal RVA  dataBase;

			public StdFields ()
			{

			}

			public StdFields (BinaryReader reader)
			{
				Read (reader);
			}

			public void Read (BinaryReader reader)
			{
				magic = reader.ReadInt16 ();
				lMajor = reader.ReadByte ();
				lMinor = reader.ReadByte ();
				codeSize = reader.ReadUInt32 ();
				initDataSize = reader.ReadUInt32 ();
				uninitDataSize = reader.ReadUInt32 ();
				entryRVA = new RVA (reader.ReadUInt32 ());
				codeBase = new RVA (reader.ReadUInt32 ());
				dataBase = new RVA (reader.ReadUInt32 ());
			}

		}
		

		/// <summary>
		/// Windows-specific fields.
		/// </summary>
		/// <remarks>
		/// See Partition II, 24.2.3.2
		/// </remarks>
		public class NTFields {
			internal uint      imgBase;
			internal uint      sectAlign;
			internal uint      fileAlign;
			internal short     osMaj;
			internal short     osMin;
			internal short     imgMaj;
			internal short     imgMin;
			internal short     subSysMaj;
			internal short     subSysMin;
			internal int       reserved_win32ver;
			internal uint      imgSize;
			internal uint      hdrSize;
			internal uint      chksum;
			internal short	   subSys;
			internal short     dllFlags;
			internal uint      stackRes;
			internal uint      stackCommit;
			internal uint      heapRes;
			internal uint      heapCommit;
			internal uint      ldrFlags;
			internal uint      numDirs;

			public NTFields ()
			{

			}
			
			public NTFields (BinaryReader reader) 
			{
				Read (reader);
			}

			public void Read (BinaryReader reader) 
			{
				imgBase = reader.ReadUInt32 ();
				sectAlign = reader.ReadUInt32 ();
				fileAlign = reader.ReadUInt32 ();
				osMaj = reader.ReadInt16 ();
				osMin = reader.ReadInt16 ();
				imgMaj = reader.ReadInt16 ();
				imgMin = reader.ReadInt16 ();
				subSysMaj = reader.ReadInt16 ();
				subSysMin = reader.ReadInt16 ();
				reserved_win32ver = reader.ReadInt32 ();
				imgSize = reader.ReadUInt32 ();
				hdrSize = reader.ReadUInt32 ();
				chksum = reader.ReadUInt32 ();
				subSys = reader.ReadInt16 ();
				dllFlags = reader.ReadInt16 ();
				stackRes = reader.ReadUInt32 ();
				stackCommit  = reader.ReadUInt32 ();
				heapRes  = reader.ReadUInt32 ();
				heapCommit  = reader.ReadUInt32 ();
				ldrFlags  = reader.ReadUInt32 ();
				numDirs  = reader.ReadUInt32 ();
			}

			public void Write (BinaryWriter writer) 
			{
				writer.Write (imgBase);
				writer.Write (sectAlign);
				writer.Write (fileAlign);
				writer.Write (osMaj);
				writer.Write (osMin);
				writer.Write (imgMaj);
				writer.Write (imgMin);
				writer.Write (subSysMaj);
				writer.Write (subSysMin);
				writer.Write (reserved_win32ver);
				writer.Write (imgSize);
				writer.Write (hdrSize);
				writer.Write (chksum);
				writer.Write ((short)subSys);
				writer.Write (dllFlags);
				writer.Write (stackRes);
				writer.Write (stackCommit);
				writer.Write (heapRes);
				writer.Write (heapCommit);
				writer.Write (ldrFlags);
				writer.Write (numDirs);
			}

			public string OSVersion {
				get {
					return String.Format("{0}.{1}", osMaj, osMin);
				}
			}

			public string ImageVersion {
				get {
					return String.Format("{0}.{1}", imgMaj, imgMin);
				}
			}

			public string SubsysVersion {
				get {
					return String.Format("{0}.{1}", subSysMaj, subSysMin);
				}
			}


			/// <summary>
			/// </summary>
			/// <returns></returns>
			public override string ToString() {
				return String.Format(
					"Image Base            : 0x{0}" + Environment.NewLine +
					"Section Alignment     : 0x{1}" + Environment.NewLine +
					"File Alignment        : 0x{2}" + Environment.NewLine +
					"OS Version            : {3}" + Environment.NewLine +
					"Image Version         : {4}" + Environment.NewLine +
					"Subsystem Version     : {5}" + Environment.NewLine +
					"Reserved/Win32Ver     : {6}" + Environment.NewLine +
					"Image Size            : {7}" + Environment.NewLine +
					"Header Size           : {8}" + Environment.NewLine +
					"Checksum              : 0x{9}" + Environment.NewLine +
					"Subsystem             : {10}" + Environment.NewLine +
					"DLL Flags             : {11}" + Environment.NewLine +
					"Stack Reserve Size    : 0x{12}" + Environment.NewLine +
					"Stack Commit Size     : 0x{13}" + Environment.NewLine +
					"Heap Reserve Size     : 0x{14}" + Environment.NewLine +
					"Heap Commit Size      : 0x{15}" + Environment.NewLine +
					"Loader Flags          : {16}" + Environment.NewLine +
					"Number of Directories : {17}" + Environment.NewLine,
					imgBase.ToString("X"), sectAlign.ToString("X"), fileAlign.ToString("X"),
					OSVersion, ImageVersion, SubsysVersion,
					reserved_win32ver,
					imgSize, hdrSize, chksum.ToString("X"), subSys, dllFlags,
					stackRes.ToString("X"), stackCommit.ToString("X"), heapRes.ToString("X"), heapCommit.ToString ("X"),
					ldrFlags, numDirs
					);
			}
		}


		internal StdFields stdFlds;
		internal NTFields ntFlds;

		internal DataDir exportDir;
		internal DataDir importDir;
		internal DataDir resourceDir;
		internal DataDir exceptionDir;
		internal DataDir securityDir;
		internal DataDir baseRelocDir;
		internal DataDir debugDir;
		internal DataDir copyrightDir;
		internal DataDir GPDir;
		internal DataDir TLSDir;
		internal DataDir loadCfgDir;
		internal DataDir boundImpDir;
		internal DataDir IATDir;
		internal DataDir delayImpDir;
		internal DataDir CLIHdrDir;
		internal DataDir reservedDir;


		public bool IsCLIImage {
			get {
				return (CLIHdrDir.virtAddr.Value != 0);
			}
		}




		/// <summary>
		/// </summary>
		public void Read(BinaryReader reader)
		{
			stdFlds = new StdFields (reader);
			ntFlds = new NTFields (reader);

			exportDir = new DataDir (reader);
			importDir = new DataDir (reader);		
			resourceDir = new DataDir (reader);
			exceptionDir = new DataDir (reader);
			securityDir = new DataDir (reader);
			baseRelocDir = new DataDir (reader);
			debugDir = new DataDir (reader);
			copyrightDir = new DataDir (reader);
			GPDir = new DataDir (reader);
			TLSDir = new DataDir (reader);
			loadCfgDir = new DataDir (reader);
			boundImpDir = new DataDir (reader);
			IATDir = new DataDir (reader);
			delayImpDir = new DataDir (reader);
			CLIHdrDir = new DataDir (reader);
			reservedDir = new DataDir (reader);
			
		}

	}

}

