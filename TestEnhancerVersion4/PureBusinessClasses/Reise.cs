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
using System.Collections;
using NDO;

namespace Reisekosten
{
	[NDOPersistent]
	public class Reise : IComparable
	{
		private string zweck;

		[NDORelation(typeof (Reisekosten.Land), RelationInfo.Default)]
		IList dieLaender = new ArrayList();

		public Reise()
		{
		}
		
		public string Zweck
		{
			get { return zweck; }
			set { zweck = value; }
		}

		public void LandHinzufügen(Land l) {
			dieLaender.Add(l);
		}

		public IList Länder {
			get {
				return dieLaender;
			}
		}

        public void LandLöschen(Land l)
        {
            dieLaender.Remove(l);
        }

		public void LandLöschen(string name) {
			Land result = null;
			foreach(Land l in dieLaender) {
				if(l.Name == name) {
					result = l;
					break;
				}
			}
			if(result != null)
				dieLaender.Remove(result);
		}

		[NDORelation(typeof (Reisekosten.Kostenpunkt), RelationInfo.Composite)]
		private IList belege = new ArrayList();

		public void AddKostenpunkt(Kostenpunkt k) {
			belege.Add(k);
		}

		public void Löschen(Kostenpunkt kp) {
			belege.Remove(kp);
		}

		public void LöscheKostenpunkte() {
			belege.Clear();
		}

		public void ErsetzeKostenpunkte() {
			belege = null;
		}

		public void ErsetzeKostenpunkte(IList belege) {
			this.belege = belege;
		}

		public IList Kostenpunkte {
			get { return belege; }
		}

		public double Gesamtkosten {
			get {
				double summe = 0;
				foreach(Kostenpunkt kp in belege) {
					summe += kp.Kosten;
				}
				return summe;
			}
		}


		public void Drucken() {
			foreach(Kostenpunkt k in belege) {
				k.Drucken();
			}
		}
		#region IComparable Member

		public int CompareTo(object obj)
		{
            Reise r2 = obj as Reise;
            if ((object)r2 == null) return -1;
            return this.zweck.CompareTo(r2.zweck);
		}

		#endregion
	}
}
