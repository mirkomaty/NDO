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

namespace Reisekosten.Personal {
	/// <summary>
	/// Zus�tzliche digitale Signatur f�r ein Zertifikat.
	/// </summary>
	[NDOPersistent]
	public class Signatur {
		private string signature;

		[NDORelation(typeof(Zertifikat), RelationInfo.Default, "Signatur")]
		private Zertifikat owner;
		
		public Signatur() {}  // wird f�r NDO laden ben�tigt.

		public Signatur(string s) {
			this.signature = s;
		}

		public string Key {
			get { return signature; }
			set { signature = value; }
		}

		public Zertifikat Owner {
			get { return owner; }
			set { owner = value; }
		}

	}
}
