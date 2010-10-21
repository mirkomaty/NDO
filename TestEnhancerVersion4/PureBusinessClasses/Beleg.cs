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

namespace Reisekosten
{
	/// <summary>
	/// Summary description for Beleg.
	/// </summary>
	
	[NDOPersistent]
	public class Beleg : Kostenpunkt
	{
		private string betrag;
		private string belegart;

		public Beleg()
		{
			this.Datum = DateTime.Now.Date;
		}

		public Beleg(string belegart, double betrag) {
			this.Datum = DateTime.Now.Date;
			this.belegart = belegart;
			this.betrag = betrag.ToString();
		}

		public override double Kosten {
			get {
				return double.Parse(betrag);
			}
		}

		public override void Drucken() {
			Console.WriteLine("Art: {0,10} {1,6:0.00C}", belegart, Kosten);
		}
	}
}
