﻿//
// Copyright (C) 2002-2010 Mirko Matytschak 
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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NETDataObjects.NDOVSPackage
{
	public class OperatingSystem
	{
		[DllImport( "kernel32.dll" )]
		static extern IntPtr GetCurrentProcess();

		[DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
		static extern IntPtr GetModuleHandle( string moduleName );

		[DllImport( "kernel32", CharSet = CharSet.Auto, SetLastError = true )]
		static extern IntPtr GetProcAddress( IntPtr hModule,
			[MarshalAs( UnmanagedType.LPStr )]string procName );

		[DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		[return: MarshalAs( UnmanagedType.Bool )]
		static extern bool IsWow64Process( IntPtr hProcess, out bool wow64Process );

		/// <summary> 
		/// The function determines whether the current operating system is a  
		/// 64-bit operating system. 
		/// </summary> 
		/// <returns> 
		/// The function returns true if the operating system is 64-bit;  
		/// otherwise, it returns false. 
		/// </returns> 
		public static bool Is64Bit
		{
			get
			{
				if ( IntPtr.Size == 8 )  // 64-bit programs run only on Win64 
				{
					return true;
				}
				else  // 32-bit programs run on both 32-bit and 64-bit Windows 
				{
					// Detect whether the current process is a 32-bit process  
					// running on a 64-bit system. 
					bool flag;
					return ((DoesWin32MethodExist( "kernel32.dll", "IsWow64Process" ) &&
						IsWow64Process( GetCurrentProcess(), out flag )) && flag);
				}
			}
		}

		public static bool Is64BitProcess
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		/// <summary> 
		/// The function determins whether a method exists in the export  
		/// table of a certain module. 
		/// </summary> 
		/// <param name="moduleName">The name of the module</param> 
		/// <param name="methodName">The name of the method</param> 
		/// <returns> 
		/// The function returns true if the method specified by methodName  
		/// exists in the export table of the module specified by moduleName. 
		/// </returns> 
		static bool DoesWin32MethodExist( string moduleName, string methodName )
		{
			IntPtr moduleHandle = GetModuleHandle( moduleName );
			if ( moduleHandle == IntPtr.Zero )
			{
				return false;
			}
			return (GetProcAddress( moduleHandle, methodName ) != IntPtr.Zero);
		}
	}
}