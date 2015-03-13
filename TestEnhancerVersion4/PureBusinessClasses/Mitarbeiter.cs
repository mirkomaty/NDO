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
using System.Collections.Generic;
using System.Diagnostics;
using Reisekosten;
using NDO;
using System.Drawing;

namespace Reisekosten.Personal
{
	[NDOPersistent]
	public class Mitarbeiter 
	{
		public Mitarbeiter()
		{
		}

		[NDORelation(typeof(Mitarbeiter))]
		IList untergebene = new ArrayList();
		public IList Untergebene
		{
			get { return untergebene; }
			set { untergebene = value; }
		}
		public void AddUntergebener(Mitarbeiter m)
		{
			untergebene.Add(m);
		}
		public void RemoveUntergebener(Mitarbeiter m)
		{
			if (untergebene.Contains(m))
				untergebene.Remove(m);
		}

		[NDORelation]
		Mitarbeiter vorgesetzter;
		public Mitarbeiter Vorgesetzter
		{
			get { return vorgesetzter; }
			set { vorgesetzter = value; }
		}

		
		string vorname = "";
		string nachname = "";
		Point position = new Point(0,0);

		public Point Position
		{
			get { return position; }
			set { position = value; }
		}

		// Buero muss vorher existieren
		[NDORelation(typeof(Buero), RelationInfo.Default)]
		private Buero meinBuero;


		// Adresse wird von Mitarbeiter verwaltet.
		[NDORelation(typeof(Adresse), RelationInfo.Composite)]
		Adresse adresse;


		// Sozialversicherungsnummer ist eindeutig einem Mitarbeiter zugeordnet.
		[NDORelation(typeof(Sozialversicherungsnummer), RelationInfo.Composite, "Versicherung")]
		private Sozialversicherungsnummer sn;


        //[NDORelation(typeof (Reise), RelationInfo.Composite)]
        //ArrayList dieReisen = new ArrayList();
        [NDORelation(RelationInfo.Composite)]
        System.Collections.Generic.List<Reise> dieReisen = new System.Collections.Generic.List<Reise>();


		// Mitarbeiter kann viele Reisebüros benutzen. Diese existieren unabh�nig vom Mitarbeiter.
		// 1:n, mit Zwischentabelle
		[NDORelation(typeof (Reisebüro))]
		IList reiseBüros = new ArrayList();

		// Mitarbeiter kann mehrere Email-Adressen haben. Diese werden mit ihm gel�scht.
		// 1:n, Komposition, mit Zwischentabelle
		[NDORelation(typeof (Email), RelationInfo.Composite)]
		IList emails = new ArrayList();

		public Adresse Adresse 
		{
			get { return adresse; }
			set { adresse = value;}
		}

		public Buero Zimmer 
		{
			get { return meinBuero; }
			set { meinBuero = value; }
		}

		public void Umziehen(string straße, string lkz, string plz, string ort) 
		{
			adresse = new Adresse();
			adresse.Straße = straße;
			adresse.Lkz = lkz;
			adresse.Plz = plz;
			adresse.Ort = ort;
		}

		public void Versichern(int nummer) 
		{
			sn = new Sozialversicherungsnummer();
			sn.SVN = nummer;
			// Note Angestellter in SVN wird automatisch gesetzt!
		}

		public Sozialversicherungsnummer SVN 
		{
			get { return sn; }
			set { sn = value; }
		}
			

		public Reise ErzeugeReise()
		{
			Reise r = new Reise();
			dieReisen.Add(r);
			return r;
		}


		public void Hinzufuegen(Email e) 
		{
			emails.Add(e);
		}

		
		public void Löschen(Email e) 
		{
			emails.Remove(e);
		}

		public void Hinzufuegen(Reisebüro r) 
		{
			reiseBüros.Add(r);
		}

		
		public void Löschen(Reisebüro r) 
		{
			reiseBüros.Remove(r);
		}


		#region ArrayListTests

        public void ReisenAddRange(ICollection c)
        {
            //Generic
            List<Reise> rl = new List<Reise>();
            foreach (Reise r in c)
                rl.Add(r);
            dieReisen.AddRange(rl);  //c
        }

		public int ReisenBinarySearch(Reise r)
		{
			return dieReisen.BinarySearch(r);
		}

		public int ReisenCapacity
		{
			get { return dieReisen.Capacity; }
		}

        public IList ReisenClone
        {
//Generic            get { return (IList)dieReisen.Clone(); }
            get { return null; }
        }

		public IList ReisenGetRange(int index, int count)
		{
			return dieReisen.GetRange(index, count);
		}

		public void ReisenInsertRange(int index, ICollection range)
		{
            //Generic
            List<Reise> rl = new List<Reise>();
            foreach (Reise r in range)
                rl.Add(r);
			dieReisen.InsertRange(index, rl); //range
		}

		public int ReisenLastIndexOf(Reise r, int index, int count)
		{
			return dieReisen.LastIndexOf(r, index, count);
		}

		public void ReisenRemoveRange(int index, int count)
		{
			dieReisen.RemoveRange(index, count);
		}

		public void ReisenReverse()
		{
			dieReisen.Reverse();
		}
//Generic
        public void ReisenSetRange(int index, ICollection c)
        {
            //dieReisen.SetRange(index, c);
        }

		public void ReisenSort()
		{
			dieReisen.Sort();
		}

		public object ReisenToArray(Type t)
		{
            //Generic
		//	return dieReisen.ToArray(t);
            return dieReisen.ToArray();
        }

        //Generic
        //public void ReisenTrimToSize()
        //{
        //    dieReisen.TrimToSize();
        //}
		
		public int ReisenCount
		{
			get { return dieReisen.Count; }
		}

		public bool ReisenContains(Reise r)
		{
			return (dieReisen.Contains(r));
		}

		public Reise[] ReisenCopyTo
		{
			get 
			{ 
				Reise[] rarr = new Reise[100];  // Don't use dieReisen.Count
				dieReisen.CopyTo(rarr, 0);
				return rarr;
			}
		}

		public bool ReisenEquals(Object o)
		{
			// Don't check the result of Equals 
			// but check the LoadState of the relation
			return dieReisen.Equals(o);
		}

		public IEnumerator ReisenEnumerator
		{
			get { return dieReisen.GetEnumerator(); }
		}

		public int ReisenHashCode()
		{
			return dieReisen.GetHashCode();
		}

		public Type ReisenGetType()
		{
			return dieReisen.GetType();
		}

		public int ReisenIndexOf(Reise r)
		{
			return dieReisen.IndexOf(r);
		}
//Generic
        //public bool ReisenIsFixedSize
        //{
        //    get { return dieReisen.IsFixedSize; }
        //}
        //public bool ReisenIsReadOnly
        //{
        //    get { return dieReisen.IsReadOnly; }
        //}
        //public bool ReisenIsSynchronized
        //{
        //    get { return dieReisen.IsSynchronized; }
        //}
        //public object ReisenSyncRoot
        //{
        //    get { return dieReisen.SyncRoot; }
        //}

		public void ReisenRemoveAt(int i) 
		{
			dieReisen.RemoveAt(i);
		}

		public string ReisenToString
		{
			get { return dieReisen.ToString(); }
		}

		#endregion






		public void Hinzufuegen(Reise r) 
		{
			dieReisen.Add(r);
		}

		public void Einfügen(Reise r, int ind) 
		{
			dieReisen.Insert(ind, r);
		}

		
		public void Löschen(Reise r) 
		{
			dieReisen.Remove(r);
		}

		public void LöscheReisen() 
		{
			dieReisen.Clear();
		}

		public void ErsetzeReisen(IList reiseListe) 
		{
            //Generic
            if (reiseListe == null)
            {
                dieReisen = null;
                return;
            }
            List<Reise> rl = new List<Reise>();
            foreach (Reise r in reiseListe)
                rl.Add(r);
			//dieReisen = (ArrayList) reiseListe;
            dieReisen = rl;
		}

		public List<Reise> Reisen 
		{
			get 
			{
				return dieReisen;
			}
		}

		public IList Reisebüros {
			get {
				return reiseBüros;
			}
		}
		public IList Emails {
			get {
				return emails;
			}
		}

		public void LöscheEmails() {
			emails.Clear();
		}

		public void LöscheReisebüros() {
			reiseBüros.Clear();
		}

		public void TestIList()
		{
			int i = 0;

			// Leseoperationen
			Reise r = (Reise) dieReisen[i];
			foreach(Reise r2 in dieReisen)
			{
				Console.WriteLine (r2.Zweck);
			}
			Reise[] arr = new Reise[]{};
			dieReisen.CopyTo(arr, 0);
			IEnumerator ie = dieReisen.GetEnumerator();


			// Schreiboperationen
			dieReisen.Add(r);
			dieReisen.Clear();
			dieReisen.Insert(0, r);
			dieReisen.Remove(r);
			dieReisen.RemoveAt(0);

			// Kann mit der blo�en Oid implementiert werden.
			bool b = dieReisen.Contains(r);
			dieReisen.IndexOf(r);
		}

		public string Vorname
		{
			get { return vorname; } 
			set { vorname = value; }
		}

		public string Nachname
		{
			get { return nachname; }
			set { nachname = value; }
		}

	}
}
