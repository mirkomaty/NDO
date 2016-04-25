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

		public void LandHinzufÃ¼gen(Land l) {
			dieLaender.Add(l);
		}

		public IList LÃ¤nder {
			get {
				return dieLaender;
			}
		}

        public void LandLÃ¶schen(Land l)
        {
            dieLaender.Remove(l);
        }

		public void LandLÃ¶schen(string name) {
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

		public void LÃ¶schen(Kostenpunkt kp) {
			belege.Remove(kp);
		}

		public void LÃ¶scheKostenpunkte() {
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
