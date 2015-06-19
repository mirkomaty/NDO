//
// Copyright (C) 2002-2015 Mirko Matytschak 
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

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Text.RegularExpressions;
using System.ComponentModel.Design;

namespace NETDataObjects.NDOVSPackage
{
    /// <summary>
    /// Zusammenfassung für MergeConflictUseCGCode.
    /// </summary>
    internal class MergeConflictUseCGCode : MergeConflictBase
    {
		public MergeConflictUseCGCode( _DTE dte, CommandID commandId )
			: base( dte, commandId )
		{
		}

        public override void DoIt()
        {
            EditPoint ep;
            int myCodeOffset;
            int cgCodeOffset;
            int endConflictOffset;
            if (!GetConflictPositions(out ep, out myCodeOffset, out cgCodeOffset, out endConflictOffset))
                return; 
            
            ep.MoveToAbsoluteOffset(endConflictOffset);
            ep.LineDown(1);
            EditPoint ep2 = ep.CreateEditPoint();
            ep.MoveToAbsoluteOffset(endConflictOffset);
            ep.Delete(ep2);
            // Delete from myCodeOffset until cgCodeOffset
            // Since cgCodeOffset starts at the beginning of the line
            // we have to move down a line
            ep.MoveToAbsoluteOffset(cgCodeOffset);
            ep.LineDown(1);
            ep2 = ep.CreateEditPoint();
            ep.MoveToAbsoluteOffset(myCodeOffset);
            ep.Delete(ep2);
        }
    }
}



