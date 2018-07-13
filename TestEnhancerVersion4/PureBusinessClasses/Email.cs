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
using Reisekosten.Personal;

namespace Reisekosten {
	/// <summary>
	/// Ein Mitarbeiter kann viele Email-Adressen haben, die jedoch gelöscht werden, wenn er geht.
	/// 1:n Komposition mit Zwischentabelle.
	/// Eine Email kann ein Zertifikat aus dem Firmenpool haben. 1:1-Aggregation bidirektional
	/// </summary>
	[NDOPersistent]
	public class Email : IPersistentObject
	{
		private string adresse;

		[NDORelation( typeof( Subject ), RelationInfo.Composite )]
		Subject subject;

		[NDORelation( typeof( Zertifikat ), RelationInfo.Default, "Zertifikat" )]
		private Zertifikat key;

		public Subject Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		public Email()
		{
		}

		public Email( string adresse )
		{
			this.adresse = adresse;
		}

		public string Adresse
		{
			get { return adresse; }
			set { adresse = value; }
		}

		public Zertifikat Schlüssel
		{
			get { return key; }
			set { key = value; }
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
