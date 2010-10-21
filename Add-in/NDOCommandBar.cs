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
using EnvDTE;
#if NET20
using EnvDTE80;
#endif
using Extensibility;
#if NET11
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für NDOCommandBar.
	/// </summary>
	internal class NDOCommandBar
	{
		private static NDOCommandBar instance = null;

#if NDO11
		public string Name = ".NET Data Objects AddIn, 1.1";
#endif
#if NDO12
		public string Name = ".NET Data Objects AddIn, 1.2";
#endif
#if NDO20
		public string Name = ".NET Data Objects AddIn, 2.0";
#endif

        _DTE applicationObject;
		private CommandBar commandBar = null;

		public static NDOCommandBar Instance
		{
			get 
			{
				return instance;
			}
		}

		public static void CreateInstance(_DTE applicationObject)
		{
            if (instance == null)
            {
                instance = new NDOCommandBar(applicationObject);
                instance.OnCreation();
            }
		}

		public CommandBar CommandBar
		{
			get
			{
				if (this.commandBar != null)
                    return this.commandBar;

                Commands commands = applicationObject.Commands;

                foreach (CommandBar bar in (CommandBars)applicationObject.CommandBars)
				{
					if ( bar.Name.Equals( Name ) )
					{
						if (this.commandBar != null) // remove multiple instances
						{
							try 
							{
								//commands.RemoveCommandBar(bar);
                                bar.Delete();
							}
							catch {}
						}
						else
						{
							this.commandBar = bar;
						}
					}
				}
                return this.commandBar;
			}
		}

		public NDOCommandBar(_DTE applicationObject)
		{
			//commandBars  = applicationObject.CommandBars;
			this.applicationObject = applicationObject;
		}

        //public void OnDisconnect()
        //{
        //    Commands commands = applicationObject.Commands;

        //    foreach (CommandBar bar in (CommandBars) applicationObject.CommandBars)
        //    {
        //        if ( bar.Name.Equals( Name ) )
        //        {
        //            try
        //            {
        //                commands.RemoveCommandBar(bar);
        //            }
        //            catch {}
        //        }
        //    }
        //}

		public void OnCreation()
		{
			if ( null == this.CommandBar)  // CommandBar != null if it exists
			{
				Commands commands = applicationObject.Commands;
				this.commandBar = (CommandBar) commands.AddCommandBar( Name, vsCommandBarType.vsCommandBarTypeToolbar, null, 0 );
				this.commandBar.Enabled = true;
				try
				{
					commandBar.Position = MsoBarPosition.msoBarTop;
                    commandBar.Visible = true;
				}
				catch  // If it doesn't work at this position, don't worry.
				{}
			}
		}
	}
}
