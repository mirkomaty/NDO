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

namespace Reisekosten.Personal {
	/// <summary>
	/// Buero eines Mitarbeiters
	/// </summary>
	[NDOPersistent]
	public class Buero : IPersistentObject
	{
		private string zimmer;
		private int nummer;
		private bool hatSchnellesInternet;
		private bool hatCat6Anschluss;
		public Buero() {}  // wird für NDO laden benötigt.

		public Buero(string zimmer)
		{
			this.zimmer = zimmer;
		}

		public string Zimmer
		{
			get { return zimmer; }
			set { zimmer = value; }
		}

		public int Nummer
		{
			get { return this.nummer; }
			set { this.nummer = value; }
		}

		public bool HatSchnellesInternet
		{
			get { return this.hatSchnellesInternet; }
			set { this.hatSchnellesInternet = value; }
		}

		public bool HatCat6Anschluss
		{
			get { return this.hatCat6Anschluss; }
			set { this.hatCat6Anschluss = value; }
		}



		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}
	}
}
