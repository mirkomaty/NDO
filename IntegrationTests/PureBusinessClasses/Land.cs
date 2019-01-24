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
using NDO;
using System.Collections;
using System.Data;

namespace Reisekosten
{
	/// <summary>
	/// Summary description for Land.
	/// </summary>
	[NDOPersistent]
	public class Land : IPersistentObject
	{
		private string name;

		[NDORelation(typeof(Reise))]
		private IList dieReisen = new ArrayList();

		private bool isInEu;

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

		public bool IsInEu
		{
			get { return this.isInEu; }
			set { this.isInEu = value; }
		}

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
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

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
