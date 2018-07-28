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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NDO;

namespace Reisekosten
{
	[NDOPersistent]
	public class Reise : IComparable, IPersistentObject
	{
		private string zweck;

		[NDORelation(RelationInfo.Default)]
		List<Land> dieLaender = new List<Land>();

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

		public List<Land> Länder {
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

		[NDORelation(RelationInfo.Composite)]
		private List<Kostenpunkt> belege = new List<Kostenpunkt>();

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

		public void ErsetzeKostenpunkte(IEnumerable<Kostenpunkt> belege) {
			this.belege = belege.ToList();
		}

		public IList<Kostenpunkt> Kostenpunkte {
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

		public NDOObjectState NDOObjectState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ObjectId NDOObjectId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid NDOTimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

		public void NDOMarkDirty()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
