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


		// Mitarbeiter kann viele ReisebÃ¼ros benutzen. Diese existieren unabhï¿½nig vom Mitarbeiter.
		// 1:n, mit Zwischentabelle
		[NDORelation(typeof (ReisebÃ¼ro))]
		IList reiseBÃ¼ros = new ArrayList();

		// Mitarbeiter kann mehrere Email-Adressen haben. Diese werden mit ihm gelï¿½scht.
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

		public void Umziehen(string straÃŸe, string lkz, string plz, string ort) 
		{
			adresse = new Adresse();
			adresse.StraÃŸe = straÃŸe;
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

		
		public void LÃ¶schen(Email e) 
		{
			emails.Remove(e);
		}

		public void Hinzufuegen(ReisebÃ¼ro r) 
		{
			reiseBÃ¼ros.Add(r);
		}

		
		public void LÃ¶schen(ReisebÃ¼ro r) 
		{
			reiseBÃ¼ros.Remove(r);
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

		public void EinfÃ¼gen(Reise r, int ind) 
		{
			dieReisen.Insert(ind, r);
		}

		
		public void LÃ¶schen(Reise r) 
		{
			dieReisen.Remove(r);
		}

		public void LÃ¶scheReisen() 
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

		public IList ReisebÃ¼ros {
			get {
				return reiseBÃ¼ros;
			}
		}
		public IList Emails {
			get {
				return emails;
			}
		}

		public void LÃ¶scheEmails() {
			emails.Clear();
		}

		public void LÃ¶scheReisebÃ¼ros() {
			reiseBÃ¼ros.Clear();
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

			// Kann mit der bloï¿½en Oid implementiert werden.
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
