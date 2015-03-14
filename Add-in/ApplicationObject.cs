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
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
using Microsoft.Win32;
namespace NDOAddIn
{
	/// <summary>
	/// Zusammenfassung für ApplicationObject.
	/// </summary>
	internal class ApplicationObject
	{
		static _DTE applicationObject;
        static string assemblyPath;

		public static _DTE VisualStudioApplication
		{
			get { return applicationObject; }
			set { applicationObject = value; }
		}

        public static string AssemblyPath
        {
            get
            {
                if (null == assemblyPath)
                {
					RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\NDO" );
                    if (key == null)
                        throw new Exception(@"Can't find NDO in the registry at HKLM\SOFTWARE\NDO. Please reinstall NDO.");
                    assemblyPath = (string)key.GetValue("InstallDir");
					if ( assemblyPath == null )
						throw new Exception( @"Can't find InstallDir value in the registry at HKLM\SOFTWARE\NDO. Please reinstall NDO." );
				}
                return assemblyPath;
            }
        }

	}
}
