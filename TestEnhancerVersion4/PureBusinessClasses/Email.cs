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
using Reisekosten.Personal;

namespace Reisekosten {
	/// <summary>
	/// Ein Mitarbeiter kann viele Email-Adressen haben, die jedoch gel�scht werden, wenn er geht.
	/// 1:n Komposition mit Zwischentabelle.
	/// Eine Email kann ein Zertifikat aus dem Firmenpool haben. 1:1-Aggregation bidirektional
	/// </summary>
	[NDOPersistent]
	public class Email {
		private string adresse;

		[NDORelation(typeof(Subject), RelationInfo.Composite)]
		Subject subject;

		[NDORelation(typeof(Zertifikat), RelationInfo.Default, "Zertifikat")]
		private Zertifikat key;

		public Subject Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		public Email() 
		{
		}

		public Email(string adresse) {
			this.adresse = adresse;
		}

		public string Adresse {
			get { return adresse; }
			set { adresse = value; }
		}

		public Zertifikat Schlüssel {
			get { return key; }
			set { key = value; }
		}

	}
}
