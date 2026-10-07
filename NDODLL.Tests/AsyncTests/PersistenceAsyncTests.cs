using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NDO;
using NDO.Query;
using NUnit.Framework;
using PureBusinessClasses;
using Reisekosten;
using Reisekosten.Personal;

namespace AsyncTests
{
	/// <summary>
	/// Tests the asynchronous code paths of NDO against a Sqlite database.
	/// </summary>
	/// <remarks>
	/// The tests use only classes with a single public constructor, because NDO currently can't
	/// materialize classes with more than one public constructor (ActivatorUtilities.CreateInstance
	/// finds more than one applicable constructor).
	/// </remarks>
	[TestFixture]
	public class PersistenceAsyncTests
	{
		SqliteTestDatabase db;

		[SetUp]
		public void SetUp()
		{
			this.db = new SqliteTestDatabase();
		}

		[TearDown]
		public void TearDown()
		{
			this.db.Dispose();
		}

		static int IdOf( object o ) => (int) ((IPersistenceCapable) o).NDOObjectId.Id[0];

		static Mitarbeiter NewMitarbeiter( string vorname, string nachname = "Test", decimal gehalt = 1000m )
		{
			return new Mitarbeiter() { Vorname = vorname, Nachname = nachname, Gehalt = gehalt };
		}

		static async Task<List<Mitarbeiter>> FindMitarbeiterAsync( PersistenceManager pm, string vorname )
		{
			var q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
			q.Parameters.Add( vorname );
			return await q.ExecuteAsync();
		}

		[Test]
		public async Task SaveAsyncInsertsAndReadsBackTheAutoincrementedId()
		{
			using (var pm = db.NewPersistenceManager())
			{
				var m = NewMitarbeiter( "Hans", "Meier", 4711m );
				pm.MakePersistent( m );
				await pm.SaveAsync();
				Assert.That( IdOf( m ), Is.GreaterThan( 0 ) );
				Assert.That( ((IPersistenceCapable) m).NDOObjectState, Is.EqualTo( NDOObjectState.Persistent ) );
			}

			using (var pm = db.NewPersistenceManager())
			{
				var result = await FindMitarbeiterAsync( pm, "Hans" );
				Assert.That( result.Count, Is.EqualTo( 1 ) );
				Assert.That( result[0].Nachname, Is.EqualTo( "Meier" ) );
				Assert.That( result[0].Gehalt, Is.EqualTo( 4711m ) );
			}
		}

		[Test]
		public void SynchronousWrappersWork()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Sync" ) );
				pm.Save();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var q = new NDOQuery<Mitarbeiter>( pm, "vorname = {0}" );
				q.Parameters.Add( "Sync" );
				Assert.That( q.Execute().Count, Is.EqualTo( 1 ) );
				Assert.That( q.ExecuteSingle( true ).Vorname, Is.EqualTo( "Sync" ) );
			}
		}

		[Test]
		public async Task UpdateAndDeleteWork()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Update", "Alt" ) );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var m = (await FindMitarbeiterAsync( pm, "Update" )).Single();
				m.Nachname = "Neu";
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var m = (await FindMitarbeiterAsync( pm, "Update" )).Single();
				Assert.That( m.Nachname, Is.EqualTo( "Neu" ) );
				pm.Delete( m );
				await pm.SaveAsync();
				Assert.That( ((IPersistenceCapable) m).NDOObjectState, Is.EqualTo( NDOObjectState.Transient ) );
			}

			using (var pm = db.NewPersistenceManager())
			{
				Assert.That( await FindMitarbeiterAsync( pm, "Update" ), Is.Empty );
			}
		}

		[Test]
		public async Task OneToManyRelationIsLoaded()
		{
			using (var pm = db.NewPersistenceManager())
			{
				var m = NewMitarbeiter( "Reisender" );
				m.ErzeugeReise().Zweck = "ADC";
				m.ErzeugeReise().Zweck = "ASW";
				pm.MakePersistent( m );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var m = (await FindMitarbeiterAsync( pm, "Reisender" )).Single();
				await pm.LoadRelationAsync( m, "dieReisen", false );
				Assert.That( m.ReisenCount, Is.EqualTo( 2 ) );
			}

			using (var pm = db.NewPersistenceManager())
			{
				// Lazy loading uses the synchronous wrappers
				var m = (await FindMitarbeiterAsync( pm, "Reisender" )).Single();
				Assert.That( m.ReisenCount, Is.EqualTo( 2 ) );
			}
		}

		[Test]
		public async Task MappingTableEntriesAreWrittenReadAndDeleted()
		{
			// Device.subdevices is a polymorphic composite relation with the mapping table relDeviceDevice
			using (var pm = db.NewPersistenceManager())
			{
				var parent = new Device() { Name = "Parent" };
				parent.NewDevice( typeof( Device ) ).Name = "Child";
				var snmp = (SnmpDevice) parent.NewDevice( typeof( SnmpDevice ) );
				snmp.Name = "Snmp";
				snmp.Port = 161;
				pm.MakePersistent( parent );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var parent = await FindParentDeviceAsync( pm );
				Assert.That( parent.Subdevices.Count, Is.EqualTo( 2 ) );
				var snmp = parent.Subdevices.OfType<SnmpDevice>().Single();
				Assert.That( snmp.Port, Is.EqualTo( 161 ) );
				parent.RemoveDevice( snmp );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var parent = await FindParentDeviceAsync( pm );
				await pm.LoadRelationAsync( parent, "subdevices", false );
				Assert.That( parent.Subdevices.Select( d => d.Name ), Is.EqualTo( new[] { "Child" } ) );
				// The composite child has been deleted together with the mapping table entry
				Assert.That( await pm.Objects<SnmpDevice>().CountAsync(), Is.EqualTo( 0 ) );
			}
		}

		static async Task<Device> FindParentDeviceAsync( PersistenceManager pm )
		{
			// The mapping of Device has no accessor names, so Linq can't be used here.
			var q = new NDOQuery<Device>( pm, "name = {0}" );
			q.AllowSubclasses = false;
			q.Parameters.Add( "Parent" );
			return await q.ExecuteSingleAsync( true );
		}

		/// <summary>
		/// Loads an object in a PersistenceManager, deletes it with another PersistenceManager
		/// and changes it in the first one, so that the update affects no rows.
		/// </summary>
		async Task<(PersistenceManager, Mitarbeiter)> PrepareCollisionAsync()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Kollision" ) );
				await pm.SaveAsync();
			}

			var pmA = db.NewPersistenceManager();
			var a = (await FindMitarbeiterAsync( pmA, "Kollision" )).Single();

			using (var pmB = db.NewPersistenceManager())
			{
				pmB.Delete( (await FindMitarbeiterAsync( pmB, "Kollision" )).Single() );
				await pmB.SaveAsync();
			}

			a.Nachname = "Geaendert";
			return (pmA, a);
		}

		[Test]
		public async Task ConcurrencyErrorIsDetected()
		{
			var (pm, _) = await PrepareCollisionAsync();
			using (pm)
			{
				Assert.ThrowsAsync<DBConcurrencyException>( async () => await pm.SaveAsync() );
			}
		}

		[Test]
		public async Task CollisionEventIsCalled()
		{
			var (pm, m) = await PrepareCollisionAsync();
			using (pm)
			{
				object collided = null;
				pm.CollisionEvent += o => collided = o;
				await pm.SaveAsync();
				Assert.That( collided, Is.SameAs( m ) );
			}
		}

		[Test]
		public async Task AggregatesVirtualTableAndAwaitForeachWork()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "A", gehalt: 100m ) );
				pm.MakePersistent( NewMitarbeiter( "B", gehalt: 300m ) );
				pm.MakePersistent( NewMitarbeiter( "C", gehalt: 200m ) );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				Assert.That( await pm.Objects<Mitarbeiter>().CountAsync(), Is.EqualTo( 3 ) );
				var max = await new NDOQuery<Mitarbeiter>( pm ).ExecuteAggregateAsync( "gehalt", AggregateType.Max );
				Assert.That( Convert.ToDecimal( max ), Is.EqualTo( 300m ) );

				var b = await pm.Objects<Mitarbeiter>().Where( m => m.Vorname == "B" ).FirstAsync();
				Assert.That( b.Gehalt, Is.EqualTo( 300m ) );
				Assert.That( await pm.Objects<Mitarbeiter>().Where( m => m.Vorname == "X" ).FirstOrDefaultAsync(), Is.Null );

				var names = new List<string>();
				await foreach (var m in pm.Objects<Mitarbeiter>().OrderBy( m => m.Vorname ))
					names.Add( m.Vorname );
				Assert.That( names, Is.EqualTo( new[] { "A", "B", "C" } ) );

				var list = await pm.Objects<Mitarbeiter>().ToListAsync();
				Assert.That( list.Count, Is.EqualTo( 3 ) );
			}
		}

		[Test]
		public async Task DeleteDirectlyAsyncWorks()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Weg" ) );
				pm.MakePersistent( NewMitarbeiter( "Bleibt" ) );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				await pm.Objects<Mitarbeiter>().Where( m => m.Vorname == "Weg" ).DeleteDirectlyAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				var all = await pm.Objects<Mitarbeiter>().ToListAsync();
				Assert.That( all.Select( m => m.Vorname ), Is.EqualTo( new[] { "Bleibt" } ) );
			}
		}

		[Test]
		public async Task DeferredCommitCanBeAborted()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				pm.MakePersistent( NewMitarbeiter( "Abort" ) );
				await pm.SaveAsync( true );
				await pm.AbortAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				Assert.That( await FindMitarbeiterAsync( pm, "Abort" ), Is.Empty );
			}
		}

		[Test]
		public async Task DeferredCommitCanBeCommitted()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				pm.MakePersistent( NewMitarbeiter( "Commit" ) );
				await pm.SaveAsync( true );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				Assert.That( (await FindMitarbeiterAsync( pm, "Commit" )).Count, Is.EqualTo( 1 ) );
			}
		}

		[Test]
		public async Task DisposeAsyncRollsBackAnOpenTransaction()
		{
			await using (var pm = db.NewPersistenceManager())
			{
				pm.TransactionMode = TransactionMode.Pessimistic;
				pm.MakePersistent( NewMitarbeiter( "Dispose" ) );
				await pm.SaveAsync( true );
			}

			using (var pm = db.NewPersistenceManager())
			{
				Assert.That( await FindMitarbeiterAsync( pm, "Dispose" ), Is.Empty );
			}
		}

		[Test]
		public async Task RefreshAsyncReloadsTheObject()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Refresh", "Alt" ) );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			using (var pm2 = db.NewPersistenceManager())
			{
				var m = (await FindMitarbeiterAsync( pm, "Refresh" )).Single();
				var m2 = (await FindMitarbeiterAsync( pm2, "Refresh" )).Single();
				m2.Nachname = "Neu";
				await pm2.SaveAsync();

				Assert.That( m.Nachname, Is.EqualTo( "Alt" ) );
				await pm.RefreshAsync( m );
				Assert.That( m.Nachname, Is.EqualTo( "Neu" ) );
			}
		}

		[Test]
		public async Task SqlPassThroughHandlerExecuteAsyncWorks()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Pass" ) );
				pm.MakePersistent( NewMitarbeiter( "Through" ) );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				await using (var handler = pm.GetSqlPassThroughHandler())
				{
					var reader = await handler.ExecuteAsync( "SELECT \"Nachname\" FROM \"Mitarbeiter\" WHERE \"Vorname\" = @p0", true, "Pass" );
					await using (reader)
					{
						Assert.That( await reader.ReadAsync(), Is.True );
						Assert.That( reader.GetString( 0 ), Is.EqualTo( "Test" ) );
						Assert.That( await reader.ReadAsync(), Is.False );
					}

					Assert.That( await handler.ExecuteAsync( "UPDATE \"Mitarbeiter\" SET \"Nachname\" = 'Neu'", false, Array.Empty<object>(), CancellationToken.None ), Is.Null );
					await handler.CommitTransactionAsync();
				}
			}

			using (var pm = db.NewPersistenceManager())
			{
				var all = await pm.Objects<Mitarbeiter>().ToListAsync();
				Assert.That( all.Select( m => m.Nachname ).Distinct(), Is.EqualTo( new[] { "Neu" } ) );
			}
		}

		[Test]
		public void CanceledTokenCancelsTheQuery()
		{
			using (var pm = db.NewPersistenceManager())
			using (var cts = new CancellationTokenSource())
			{
				cts.Cancel();
				Assert.CatchAsync<OperationCanceledException>( async () => await new NDOQuery<Mitarbeiter>( pm ).ExecuteAsync( cts.Token ) );
			}
		}

		[Test]
		public async Task ConcurrentUseOfAPersistenceManagerIsDetected()
		{
			using (var pm = db.NewPersistenceManager())
			{
				Exception caught = null;
				pm.OnSavingEvent += l =>
				{
					// Simulate another thread, which doesn't share the logical flow of the running Save operation
					Task t;
					using (ExecutionContext.SuppressFlow())
						t = Task.Run( () => pm.Objects<Mitarbeiter>().ToListAsync() );
					try
					{
						t.GetAwaiter().GetResult();
					}
					catch (Exception ex)
					{
						caught = ex;
					}
				};

				pm.MakePersistent( NewMitarbeiter( "Guard" ) );
				await pm.SaveAsync();

				Assert.That( caught, Is.InstanceOf<NDOException>() );
				Assert.That( ((NDOException) caught).ErrorNumber, Is.EqualTo( 122 ) );
			}
		}

		[Test]
		public async Task NestedCallsInCallbacksAreAllowed()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Vorhanden" ) );
				await pm.SaveAsync();
			}

			using (var pm = db.NewPersistenceManager())
			{
				int countInCallback = -1;
				pm.OnSavingEvent += l => countInCallback = pm.Objects<Mitarbeiter>().ResultTable.Count;
				pm.MakePersistent( NewMitarbeiter( "Neu" ) );
				await pm.SaveAsync();
				Assert.That( countInCallback, Is.EqualTo( 1 ) );
			}
		}

		[Test]
		public async Task GuardIsReleasedAfterAnException()
		{
			using (var pm = db.NewPersistenceManager())
			{
				Assert.CatchAsync<Exception>( async () => await new NDOQuery<Mitarbeiter>( pm, "this is not ( valid" ).ExecuteAsync() );
				Assert.That( await new NDOQuery<Mitarbeiter>( pm ).ExecuteAsync(), Is.Empty );
			}
		}

		[Test]
		public async Task ParallelPersistenceManagersDontInterfere()
		{
			using (var pm = db.NewPersistenceManager())
			{
				pm.MakePersistent( NewMitarbeiter( "Parallel" ) );
				await pm.SaveAsync();
			}

			var tasks = Enumerable.Range( 0, 4 ).Select( _ => Task.Run( async () =>
			{
				using (var pm = db.NewPersistenceManager())
					return (await FindMitarbeiterAsync( pm, "Parallel" )).Count;
			} ) ).ToArray();
			var counts = await Task.WhenAll( tasks );
			Assert.That( counts, Is.All.EqualTo( 1 ) );
		}
	}

	/// <summary>
	/// Tests the code path for providers without insert batches (e.g. Postgres):
	/// The autoincremented id is read with a separate statement within the transaction.
	/// </summary>
	[TestFixture]
	public class NoInsertBatchTests
	{
		SqliteTestDatabase db;

		[SetUp]
		public void SetUp()
		{
			this.db = new SqliteTestDatabase( "SqliteNoBatch" );
		}

		[TearDown]
		public void TearDown()
		{
			this.db.Dispose();
		}

		[Test]
		public async Task AutoincrementedIdsAreReadAfterInsert()
		{
			Mitarbeiter m1, m2;
			using (var pm = db.NewPersistenceManager())
			{
				Assert.That( pm.NDOMapping.Connections.First().Type, Is.EqualTo( "SqliteNoBatch" ) );
				m1 = new Mitarbeiter() { Vorname = "Eins" };
				m2 = new Mitarbeiter() { Vorname = "Zwei" };
				pm.MakePersistent( m1 );
				pm.MakePersistent( m2 );
				await pm.SaveAsync();

				var id1 = (int) ((IPersistenceCapable) m1).NDOObjectId.Id[0];
				var id2 = (int) ((IPersistenceCapable) m2).NDOObjectId.Id[0];
				Assert.That( id1, Is.GreaterThan( 0 ) );
				Assert.That( id2, Is.GreaterThan( 0 ) );
				Assert.That( id1, Is.Not.EqualTo( id2 ) );
			}

			using (var pm = db.NewPersistenceManager())
			{
				var q = new NDOQuery<Mitarbeiter>( pm, "oid = {0}" );
				q.Parameters.Add( ((IPersistenceCapable) m2).NDOObjectId.Id[0] );
				Assert.That( (await q.ExecuteSingleAsync( true )).Vorname, Is.EqualTo( "Zwei" ) );
			}
		}
	}
}
