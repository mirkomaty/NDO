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
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using NDO;
using NDO.Query;
using Reisekosten;

namespace NdoUnitTests
{
	/// <summary>
	/// Combined Tests of Land and Flughafen to test uniqueness of object ids.
	/// </summary>
	[TestFixture]
	public class LandFlughafenTests
	{
		public LandFlughafenTests()
		{
		}

		private PersistenceManager pm;
		private Land l;
		private Flughafen f;

		[SetUp]
		public void Setup() 
		{
			pm = PmFactory.NewPersistenceManager();
			l = CreateLand("Deutschland");
			f = CreateFlughafen("MUC");
		}

		[TearDown]
		public void TearDown() 
		{
			pm.Abort();
			IList l = pm.GetClassExtent(typeof(Land));
			pm.Delete(l);
			l = pm.GetClassExtent(typeof(Flughafen));
			pm.Delete(l);
			pm.Save();
			pm.Close();
		}

		[Test]
		public void TestCreateObjects() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			if (!pm.HasOwnerCreatedIds)
			{
				if (l.NDOObjectId.Id[0] is Int32)
					Assert.That(-1 == (int) l.NDOObjectId.Id[0], "Land key wrong");
				if (f.NDOObjectId.Id[0] is Int32)
					Assert.That(-1 == (int) f.NDOObjectId.Id[0], "Flughafen key wrong");
			}
			Assert.That(!l.NDOObjectId.Equals(f.NDOObjectId), "Ids should be different");
			l = (Land)pm.FindObject(l.NDOObjectId);
			f = (Flughafen)pm.FindObject(f.NDOObjectId);
		}

		[Test]
		public void TestCreateObjectsSave() 
		{
			
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			Assert.That(!l.NDOObjectId.Equals(f.NDOObjectId), "Ids should be different");
			l = (Land)pm.FindObject(l.NDOObjectId);
			f = (Flughafen)pm.FindObject(f.NDOObjectId);
			Assert.That(l != null, "1. Land not found");
			Assert.That(f != null, "1. Flughafen not found");

			pm.UnloadCache();
			l = (Land)pm.FindObject(l.NDOObjectId);
			f = (Flughafen)pm.FindObject(f.NDOObjectId);
			Assert.That(l != null, "2. Land not found");
			Assert.That(f != null, "2. Flughafen not found");
		}

		[Test]
		public void TestCreateObjectsSaveReload()
		{

			pm.MakePersistent( l );
			pm.MakePersistent( f );
			l.AddFlughafen( f );
			pm.Save();
			pm.UnloadCache();
			Land l2 = new NDOQuery<Land>(pm).ExecuteSingle();
			Assert.That(l2  != null);
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects" );
		}

		[Test]
		public void TestAddObjectSave() 
		{
			pm.MakePersistent(l);
			pm.Save();
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			Assert.That(NDOObjectState.Created ==  f.NDOObjectState, "1. Wrong state");
			pm.Save();
			l = (Land)pm.FindObject(l.NDOObjectId);
			f = (Flughafen)pm.FindObject(f.NDOObjectId);
			Assert.That(l != null, "1. Land not found");
			Assert.That(f != null, "1. Flughafen not found");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "2. Wrong state");
		}
			
		[Test]
		public void TestAddObjectAbort() 
		{
			pm.MakePersistent(l);
			pm.Save();
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			Assert.That(NDOObjectState.Created ==  f.NDOObjectState, "1. Wrong state");
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  f.NDOObjectState, "2. Wrong state");
			Assert.That(0 ==  l.Flughäfen.Count, "2. Wrong number of objects");
		}

		[Test]
		public void TestRemoveObjectSave() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			l.RemoveFlughafen(f);
			// f keeps the foreign key - so it get's dirty
			Assert.That(NDOObjectState.PersistentDirty ==  f.NDOObjectState, "1. Wrong state");
			Assert.That(0 ==  l.Flughäfen.Count, "2. Wrong number of objects");
			pm.Save();
			Assert.That(0 ==  l.Flughäfen.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestRemoveObjectSaveReload() 
		{
			pm.MakePersistent( l );
			pm.MakePersistent( f );
			l.AddFlughafen( f );
			pm.Save();
			pm.UnloadCache();
			NDOQuery<Land> q = new NDOQuery<Land>( pm );
			l = q.ExecuteSingle(true);
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "1. Wrong state");
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			f = (Flughafen) l.Flughäfen[0];
			l.RemoveFlughafen(f);
			// f keeps the foreign key - so it get's dirty
			Assert.That(NDOObjectState.PersistentDirty ==  f.NDOObjectState, "2. Wrong state");
			Assert.That(0 ==  l.Flughäfen.Count, "2. Wrong number of objects");
			pm.Save();
			l = (Land) q.ExecuteSingle(true);
			pm.UnloadCache();
			Assert.That(0 ==  l.Flughäfen.Count, "3. Wrong number of objects");
		}

		[Test]
		public void TestRemoveObjectSaveReloadNonHollow() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Land>(pm, null, false);
			l = (Land) q.ExecuteSingle(true);
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "1. Wrong state");
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			f = (Flughafen) l.Flughäfen[0];
			l.RemoveFlughafen(f);
			// f keeps the foreign key - so it get's dirty
			Assert.That(NDOObjectState.PersistentDirty ==  f.NDOObjectState, "2. Wrong state");
			Assert.That(0 ==  l.Flughäfen.Count, "2. Wrong number of objects");
			pm.Save();
			l = (Land) q.ExecuteSingle(true);
			pm.UnloadCache();
			Assert.That(0 ==  l.Flughäfen.Count, "3. Wrong number of objects");
		}

		[Test]
		public void TestRemoveObjectAbort() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			l.RemoveFlughafen(f);
			Assert.That(NDOObjectState.PersistentDirty ==  f.NDOObjectState, "1. Wrong state");
			Assert.That(0 ==  l.Flughäfen.Count, "2. Wrong number of objects");
			pm.Abort();
			Assert.That(1 ==  l.Flughäfen.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestDeleteSave() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.Delete(l); // sollte Exception geben
			Assert.That(NDOObjectState.Deleted ==  l.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.PersistentDirty ==  f.NDOObjectState, "2. Wrong state");
			pm.Save();
			Assert.That(NDOObjectState.Transient ==  l.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "2. Wrong state");
		}



		[Test]
		public void TestDeleteAbort() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.Delete(l);  // sollte Exception geben
			Assert.That(NDOObjectState.Deleted ==  l.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.PersistentDirty ==  f.NDOObjectState, "2. Wrong state");
			pm.Abort();
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "1. Wrong state");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveSave() 
		{
			pm.MakePersistent(l);
			pm.Save();
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			l.RemoveFlughafen(f);
			Assert.That(NDOObjectState.Created ==  f.NDOObjectState, "1. Wrong state");
			pm.Save();
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestAddRemoveAbort() 
		{
			pm.MakePersistent(l);
			pm.Save();
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			l.RemoveFlughafen(f);
			Assert.That(NDOObjectState.Created ==  f.NDOObjectState, "1. Wrong state");
			pm.Abort();
			Assert.That(NDOObjectState.Transient ==  f.NDOObjectState, "2. Wrong state");
		}

		[Test]
		public void TestClearRelatedObjectsSave() 
		{
			pm.MakePersistent(l);
			for(int i = 0; i < 10; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			IList ff = new ArrayList(l.Flughäfen);
			l.LöscheFlughäfen();
			Assert.That(0 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) 
			{
				Assert.That(NDOObjectState.PersistentDirty ==  ((Flughafen)ff[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.That(0 ==  l.Flughäfen.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) 
			{
				Assert.That(NDOObjectState.Persistent ==  ((Flughafen)ff[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestClearRelatedObjectsAbort() 
		{
			pm.MakePersistent(l);
			for(int i = 0; i < 10; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			IList ff = new ArrayList(l.Flughäfen);
			l.LöscheFlughäfen();
			Assert.That(0 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 10; i++) 
			{
				Assert.That(NDOObjectState.PersistentDirty ==  ((Flughafen)ff[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.That(10 ==  l.Flughäfen.Count, "3. Wrong number of objects");
			for(int i = 0; i < 10; i++) 
			{
				Assert.That(NDOObjectState.Persistent ==  ((Flughafen)ff[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsNullSave() 
		{
			pm.MakePersistent(l);
			for(int i = 0; i < 3; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			IList ff = new ArrayList(l.Flughäfen);
			l.ErsetzeFlughäfen(null);
			Assert.That(l.Flughäfen == null, "Flughäfen should be null");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.PersistentDirty ==  ((Flughafen)ff[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Save();
			Assert.That(l.Flughäfen == null, "Flughäfen should be null");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.Persistent ==  ((Flughafen)ff[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsNullAbort() 
		{
			pm.MakePersistent(l);
			for(int i = 0; i < 3; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			IList ff = new ArrayList(l.Flughäfen);
			l.ErsetzeFlughäfen(null);
			Assert.That(l.Flughäfen == null, "No objects should be there");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.PersistentDirty ==  ((Flughafen)ff[i]).NDOObjectState, "2. Wrong state");
			}
			pm.Abort();
			Assert.That(3 ==  l.Flughäfen.Count, "3. Wrong number of objects");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.Persistent ==  ((Flughafen)ff[i]).NDOObjectState, "4. Wrong state");
			}
		}

		[Test]
		public void TestAssignRelatedObjectsSave() 
		{
			pm.MakePersistent(l);
			for(int i = 0; i < 3; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			IList neueFlughäfen = new ArrayList();
			Flughafen nr = CreateFlughafen("Test");
			pm.MakePersistent(nr);
			neueFlughäfen.Add(nr);

			IList ff = new ArrayList(l.Flughäfen);
			l.ErsetzeFlughäfen(neueFlughäfen);
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.PersistentDirty ==  ((Flughafen)ff[i]).NDOObjectState, "2. Wrong state");
			}
			Assert.That(NDOObjectState.Created ==  nr.NDOObjectState, "3. Wrong state");

			pm.Save();
			Assert.That(1 ==  l.Flughäfen.Count, "4. Wrong number of objects");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.Persistent ==  ((Flughafen)ff[i]).NDOObjectState, "5. Wrong state");
			}
			Assert.That(NDOObjectState.Persistent ==  nr.NDOObjectState, "6. Wrong state");
		}
/*
		[Test]
		public void TestAssignRelatedObjectsAbort() 
		{
			pm.MakePersistent(l);
			for(int i = 0; i < 3; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString);
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			IList neueFlughäfen = new ArrayList();
			Flughafen nr = CreateFlughafen("Test");
			neueFlughäfen.Add(nr);

			IList ff = new ArrayList(l.Flughäfen);
			l.ErsetzeFlughafenn(neueFlughäfen);
			Assert.That(1 ==  l.Flughäfen.Count, "1. Wrong number of objects");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.Deleted ==  ((Flughafen)ff[i]).NDOObjectState, "2. Wrong state");
			}
			Assert.That(NDOObjectState.Created ==  nr.NDOObjectState, "3. Wrong state");

			pm.Abort();
			Assert.That(3 ==  l.Flughäfen.Count, "4. Wrong number of objects");
			for(int i = 0; i < 3; i++) 
			{
				Assert.That(NDOObjectState.Persistent ==  ((Flughafen)ff[i]).NDOObjectState, "5. Wrong state");
			}
			Assert.That(NDOObjectState.Transient ==  nr.NDOObjectState, "6. Wrong state");
		}


		[Test]
		public void TestHollow() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.MakeHollow(l);
			Assert.That(NDOObjectState.Hollow ==  l.NDOObjectState, "1: Land should be hollow");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "1: Flughafen should be persistent");
			IList Flughafen = l.Flughäfen;

			pm.MakeHollow(l, true);
			Assert.That(NDOObjectState.Hollow ==  l.NDOObjectState, "2: Land should be hollow");
			Assert.That(NDOObjectState.Hollow ==  f.NDOObjectState, "2: Flughafen should be hollow");

			Flughafen = l.Flughäfen;
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "3: Land should be persistent");
			Assert.That(NDOObjectState.Hollow ==  f.NDOObjectState, "3: Flughafen should be hollow");
			Assert.That("ADC" ==  f.Zweck, "3: Flughafen should have correct Zweck");
			Assert.That(NDOObjectState.Persistent ==  f.NDOObjectState, "4: Flughafen should be persistent");
		}

		[Test]
		public void  TestMakeAllHollow() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(m);
			l.AddFlughafen(f);
			pm.Save();
			pm.MakeAllHollow();
			Assert.That(NDOObjectState.Hollow ==  l.NDOObjectState, "1: Land should be hollow");
			Assert.That(NDOObjectState.Hollow ==  f.NDOObjectState, "1: Flughafen should be hollow");
			pm.UnloadCache();
		}

		[Test]
		public void  TestMakeAllHollowUnsaved() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.MakeAllHollow();  // before save, objects cannot be made hollow. => in locked objects
			Assert.That(NDOObjectState.Created ==  l.NDOObjectState, "1: Land should be created");
			Assert.That(NDOObjectState.Created ==  f.NDOObjectState, "1: Flughafen should be created");
		}

		[Test]
		public void TestLoadRelatedObjects() 
		{
			IQuery q = new NDOQuery<Land>(pm, null);
			IList dellist = q.Execute();
			pm.Delete(dellist);
			pm.Save();
			pm.UnloadCache();

			for(int i = 0; i < 10; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.MakePersistent(l);
			pm.Save();
			pm.MakeHollow(l, true);

			IList Flughafenn = new ArrayList(l.Flughäfen);
			Assert.That(10 ==  Flughafenn.Count, "Array size should be 10");

			for(int i = 0; i < 10; i++) 
			{
				Flughafen ff = (Flughafen)Flughafenn[i];
				Assert.That(NDOObjectState.Hollow ==  ff.NDOObjectState, "1: Flughafen should be hollow");
				//				if (i.ToString()!= ff.Zweck)
				//				{
				//					for (int j = 0; j < 10; j++)
				//						System.Diagnostics.Debug.WriteLine("Flughafen: " + ((Flughafen)Flughafenn[j]).Zweck);
				//				}
#if !ORACLE && !MYSQL && !FIREBIRD
				if (ff.NDOObjectId.Id[0] is Int32)
					Assert.That(i.ToString() ==  ff.Zweck, "2: Flughafen should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Flughafenn2 = l.Flughäfen;
			for(int i = 0; i < 10; i++) 
			{
				Flughafen r1 = (Flughafen)Flughafenn[i];
				Flughafen r2 = (Flughafen)Flughafenn2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				if (r1.NDOObjectId.Id[0] is Int32)
					Assert.That(i.ToString() ==  r1.Zweck, "3: Flughafen should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestLoadRelatedObjectsSave() 
		{
			pm.MakePersistent(l);
			pm.Save();
			for(int i = 0; i < 10; i++) 
			{
				Flughafen f = CreateFlughafen(i.ToString());
				pm.MakePersistent(f);
				l.AddFlughafen(f);
			}
			pm.Save();
			pm.MakeHollow(l, true);

			IList Flughafenn = new ArrayList(l.Flughäfen);

			for(int i = 0; i < 10; i++) 
			{
				Flughafen ff = (Flughafen)Flughafenn[i];
				Assert.That(NDOObjectState.Hollow ==  ff.NDOObjectState, "1: Flughafen should be hollow");
#if !ORACLE && !MYSQL && !FIREBIRD
				if (ff.NDOObjectId.Id[0] is Int32)
					Assert.That(i.ToString() ==  ff.Zweck, "2: Flughafen should be in right order");
#endif
			}


			pm.MakeAllHollow();
			pm.UnloadCache();
			IList Flughafenn2 = l.Flughäfen;
			for(int i = 0; i < 10; i++) 
			{
				Flughafen r1 = (Flughafen)Flughafenn[i];
				Flughafen r2 = (Flughafen)Flughafenn2[i];
#if !ORACLE && !MYSQL && !FIREBIRD
				if (r1.NDOObjectId.Id[0] is Int32)
					Assert.That(i.ToString() ==  r1.Zweck, "3: Flughafen should be in right order");
#endif
				Assert.That(r1 !=  r2, "Objects should be different");
			}
		}

		[Test]
		public void TestExtentRelatedObjects() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			IList liste = pm.GetClassExtent(typeof(Land));
			l = (Land)liste[0];
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "1: Land should be persistent");
			Assert.That(l.Flughäfen != null, "2. Relation is missing");
			Assert.That(1 ==  l.Flughäfen.Count, "3. Wrong number of objects");
			Assert.That(NDOObjectState.Persistent ==  ((Flughafen)l.Flughäfen[0]).NDOObjectState, "4.: Flughafen should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Land));
			l = (Land)liste[0];
			Assert.That(NDOObjectState.Hollow ==  l.NDOObjectState, "5: Land should be hollow");
			Assert.That(l.Flughäfen != null, "6. Relation is missing");
			Assert.That(1 ==  l.Flughäfen.Count, "7. Wrong number of objects");
			Assert.That(NDOObjectState.Hollow ==  ((Flughafen)l.Flughäfen[0]).NDOObjectState, "8.: Flughafen should be hollow");

			pm.UnloadCache();
			liste = pm.GetClassExtent(typeof(Land), false);
			l = (Land)liste[0];
			Assert.That(NDOObjectState.Persistent ==  l.NDOObjectState, "9: Land should be persistent");
			Assert.That(l.Flughäfen != null, "10. Relation is missing");
			Assert.That(1 ==  l.Flughäfen.Count, "11. Wrong number of objects");
			Assert.That(NDOObjectState.Hollow ==  ((Flughafen)l.Flughäfen[0]).NDOObjectState, "12.: Flughafen should be hollow");
		}

		[Test]
		public void RelationEmptyAssignment()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();
			l = (Land) pm.FindObject(l.NDOObjectId);
			//Assert.That(l.Flughäfen.Count == 1);
			l.ErsetzeFlughafenn(new ArrayList());
			pm.Save();
			pm.UnloadCache();
			l = (Land) pm.FindObject(l.NDOObjectId);
Assert.That(l.Flughäfen == null, "Flughafenn should be null");
			Assert.That(l.Flughäfen.Count == 0, "Flughafenn should be empty");
		}

		[Test]
		public void RelationNullAssignment()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();
			l = (Land) pm.FindObject(l.NDOObjectId);
			//Assert.That(l.Flughäfen.Count == 1);
			l.ErsetzeFlughafenn(null);
			pm.Save();
			pm.UnloadCache();
			l = (Land) pm.FindObject(l.NDOObjectId);
Assert.That(l.Flughäfen == null, "Flughafenn should be null");
			Assert.That(l.Flughäfen.Count == 0, "Flughafenn should be empty");
		}

#if !MYSQL
		[Test]
		public void AbortedTransaction()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();

			pm.TransactionMode = TransactionMode.Optimistic;

			IQuery q = new NDOQuery<Land>(pm, null);
			l = (Land) q.ExecuteSingle(true);
			l.Vorname = "Hans";
			((Flughafen)l.Flughäfen[0]).Zweck = "Neuer Zweck";

			FieldInfo fi = pm.NDOMapping.GetType().GetField("persistenceHandler", BindingFlags.Instance | BindingFlags.NonPublic);
			Hashtable ht = (Hashtable) fi.GetValue(pm.NDOMapping);
			ht.Clear();
			pm.NDOMapping.FindClass(typeof(Flughafen)).FindField("zweck").ColumnName = "nix";
			
			try
			{
				pm.Save();
			}
			catch
			{
				ht.Clear();
				pm.NDOMapping.FindClass(typeof(Flughafen)).FindField("zweck").ColumnName = "Zweck";
				pm.Abort();
			}
			pm.UnloadCache();
			l = (Land) q.ExecuteSingle(true);
			Assert.That("Hartmut" ==  l.Vorname, "Vorname falsch");
			Assert.That("ADC" ==  ((Flughafen)l.Flughäfen[0]).Zweck, "Vorname falsch");
			
		}
#endif

		[Test]
		public void TestListEnumerator()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Land>(pm, null);
			l = (Land) q.ExecuteSingle(true);
			IEnumerator ie = l.Flughäfen.GetEnumerator();
			bool result = ie.MoveNext();
			Assert.That(result, "Enumerator should give a result");
			Assert.That(ie.Current != null);
		}
*/

		[Test]
		public void TestListCount()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Land>(pm, null);
			l = (Land) q.ExecuteSingle(true);
			Assert.That(1 ==  l.Flughäfen.Count, "Count should be 1");
		}


		[Test]
		public void TestAddFlughafen()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.UnloadCache();
			IQuery q = new NDOQuery<Land>(pm, null);
			l = (Land) q.ExecuteSingle(true);
			Assert.That(l != null, "Land not found");
			Assert.That(1 ==  l.Flughäfen.Count, "Count wrong");
			Flughafen f2 = CreateFlughafen("FRA");
			pm.MakePersistent(f2);
			l.AddFlughafen(f2);
			pm.VerboseMode = true;
			pm.Save();
			pm.VerboseMode = false;
			pm.UnloadCache();
			l = (Land) q.ExecuteSingle(true);
			Assert.That(2 ==  l.Flughäfen.Count, "Count wrong");
		}

		public void TestAddFlughafen2()
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			pm.Save();
			l.AddFlughafen(f);
			pm.VerboseMode = true;
			pm.Save();
			pm.VerboseMode = false;
			pm.UnloadCache();
			l = (Land) pm.FindObject(l.NDOObjectId);
			Assert.That(1 ==  l.Flughäfen.Count, "Count wrong");
		}

		[Test]
		public void TestRelatedObject() 
		{
			pm.MakePersistent(l);
			pm.MakePersistent(f);
			l.AddFlughafen(f);
			pm.Save();
			pm.MakeHollow(f);
			f.Kürzel = "XXX";
			pm.Save();  // Bug would overwrite foreign key in database.
			Assert.That("XXX" ==  f.Kürzel, "Kürzel should be changed");
			ObjectId id = l.NDOObjectId;
			pm.UnloadCache();
			l = (Land) pm.FindObject(id);
			Assert.That(1 ==  l.Flughäfen.Count, "Number of children");
		}

		private Land CreateLand(string name) 
		{
			Land l = new Land();
			l.Name = name;
			return l;
		}

		private Flughafen CreateFlughafen(string name) 
		{
			Flughafen f = new Flughafen();
			f.Kürzel = name;
			return f;
		}
	}
}
