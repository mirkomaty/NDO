﻿//
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
		IList flughÃ¤fen = new ArrayList();
		public IList FlughÃ¤fen
		{
			get { return flughÃ¤fen; }
			set { flughÃ¤fen = value; }
		}
		public void AddFlughafen(Flughafen f)
		{
			flughÃ¤fen.Add(f);
		}
		public void RemoveFlughafen(Flughafen f)
		{
			if (flughÃ¤fen.Contains(f))
				flughÃ¤fen.Remove(f);
		}
		public void LÃ¶scheFlughÃ¤fen()
		{
			this.flughÃ¤fen.Clear();
		}
		public void ErsetzeFlughÃ¤fen(IList l)
		{
			this.flughÃ¤fen = l;
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
