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
using NDO;
using System.Collections;

namespace Reisekosten
{
	/// <summary>
	/// Summary description for Land.
	/// </summary>
	[NDOPersistent]
	public class Land
	{
		private string name;

		[NDORelation(typeof(Reise))]
		private IList dieReisen = new ArrayList();
		public IList DieReisen
		{
			get { return dieReisen == null ? null : ArrayList.ReadOnly(dieReisen); }
			set { dieReisen = value; }
		}
		public void AddReise(Reise r)
		{
			dieReisen.Add(r);
		}
		public void RemoveReise(Reise r)
		{
			if (dieReisen.Contains(r))
				dieReisen.Remove(r);
		}

		// Assoziation 1:n
		[NDORelation(typeof(Flughafen))]
		IList flughäfen = new ArrayList();
		public IList Flughäfen
		{
			get { return flughäfen; }
			set { flughäfen = value; }
		}
		public void AddFlughafen(Flughafen f)
		{
			flughäfen.Add(f);
		}
		public void RemoveFlughafen(Flughafen f)
		{
			if (flughäfen.Contains(f))
				flughäfen.Remove(f);
		}
		public void LöscheFlughäfen()
		{
			this.flughäfen.Clear();
		}
		public void ErsetzeFlughäfen(IList l)
		{
			this.flughäfen = l;
		}

		public Land()
		{
		}

		public Land(string name) {
			this.name = name;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
