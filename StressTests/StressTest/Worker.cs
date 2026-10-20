using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NDO;
using NDO.Linq;
using StressTestClasses;

namespace StressTest
{
	/// <summary>
	/// Thrown if the data read from the database doesn't match the expected data.
	/// </summary>
	public class VerificationException : Exception
	{
		public VerificationException( string message ) : base( message )
		{
		}
	}

	/// <summary>
	/// A worker runs the test scenario in a loop. All database operations use the async API of NDO.
	/// </summary>
	/// <remarks>
	/// Each step uses a new PersistenceManager, so that the data is really read from the database
	/// and not from the cache of a PersistenceManager. The data of a worker is identified by the run id,
	/// the worker number and the iteration, so that the workers don't interfere with each other, except
	/// for the tags, which are shared by all workers.
	/// </remarks>
	public class Worker
	{
		readonly int workerNo;
		readonly TestDatabase db;
		readonly StressTestOptions options;
		readonly Statistics statistics;
		readonly int[] tagIds;
		readonly string runId;
		readonly Random random;

		public Worker( int workerNo, TestDatabase db, StressTestOptions options, Statistics statistics, int[] tagIds, string runId )
		{
			this.workerNo = workerNo;
			this.db = db;
			this.options = options;
			this.statistics = statistics;
			this.tagIds = tagIds;
			this.runId = runId;
			this.random = new Random( workerNo );
		}

		static void Check( bool condition, string message )
		{
			if (!condition)
				throw new VerificationException( message );
		}

		static int IdOf( object o ) => (int) ((IPersistenceCapable) o).NDOObjectId.Id[0];

		/// <summary>
		/// Runs the iterations until the number of iterations is reached, the time is up or cancellation is requested.
		/// </summary>
		/// <param name="endTime">If not null, the worker runs until this time instead of a fixed number of iterations.</param>
		/// <param name="ct"></param>
		public async Task RunAsync( DateTime? endTime, CancellationToken ct )
		{
			for (int iteration = 0; ; iteration++)
			{
				if (ct.IsCancellationRequested)
					break;
				if (endTime.HasValue ? DateTime.UtcNow >= endTime.Value : iteration >= this.options.Iterations)
					break;

				var state = new IterationState( $"{this.runId}-{this.workerNo}-{iteration}" );
				try
				{
					await RunIterationAsync( state, ct );
					this.statistics.IterationSucceeded();
				}
				catch (OperationCanceledException) when (ct.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					this.statistics.IterationFailed( new Statistics.ErrorInfo { Worker = this.workerNo, Iteration = iteration, Step = state.Step, Exception = ex } );
					if (this.options.StopOnFirstError)
						throw;
				}
			}
		}

		class IterationState
		{
			public IterationState( string key )
			{
				OrderNumber = "O-" + key;
				CustomerName = "C-" + key;
			}

			public string Step;
			public readonly string OrderNumber;
			public readonly string CustomerName;
			public int OrderId;
			public HashSet<int> ExpectedTagIds;
			public int ExpectedLineCount;
			public decimal ExpectedPriceSum;
			public decimal ExpectedTotal;
		}

		async Task RunIterationAsync( IterationState s, CancellationToken ct )
		{
			await RunStepAsync( s, "Create", () => CreateAsync( s, ct ), ct );
			await RunStepAsync( s, "Read and change", () => ChangeAsync( s, ct ), ct );
			await RunStepAsync( s, "Verify", () => VerifyAsync( s, ct ), ct );
			await RunStepAsync( s, "Query", () => QueryAsync( ct ), ct );
			await RunStepAsync( s, "Delete", () => DeleteOrderAsync( s, ct ), ct );
			await RunStepAsync( s, "Delete", () => DeleteCustomerAsync( s, ct ), ct );
			await RunStepAsync( s, "Verify deletion", () => VerifyDeletionAsync( s, ct ), ct );
		}

		/// <summary>
		/// Runs a step and repeats it, if the database chose its transaction as deadlock victim.
		/// </summary>
		/// <remarks>
		/// Deadlocks can't be avoided completely with databases which use locks. The database rolls back the
		/// transaction of the victim, so the step can be repeated. Each step uses its own PersistenceManager
		/// and changes the IterationState only after a successful save, so a repetition starts from scratch.
		/// </remarks>
		async Task RunStepAsync( IterationState s, string step, Func<Task> action, CancellationToken ct )
		{
			s.Step = step;
			for (int attempt = 1; ; attempt++)
			{
				try
				{
					await action();
					return;
				}
				catch (Exception ex) when (attempt <= this.options.DeadlockRetries && !ct.IsCancellationRequested && IsDeadlock( ex ))
				{
					this.statistics.DeadlockRetried( step );
					// A random delay, so that the competing transactions don't collide again
					await Task.Delay( this.random.Next( 10, 50 ) * attempt, ct );
				}
			}
		}

		/// <summary>
		/// Checks, if an exception was caused by a deadlock.
		/// </summary>
		/// <remarks>
		/// NDO wraps the exceptions of the ADO.NET providers in NDOExceptions. Versions up to 6.0.0 don't pass the original
		/// exception as inner exception. Therefore the messages are checked as well. All supported databases mention "deadlock"
		/// in their messages (SqlServer 1205, MySql 1213, Postgre 40P01). Sqlite doesn't have deadlocks.
		/// </remarks>
		static bool IsDeadlock( Exception ex )
		{
			for (Exception e = ex; e != null; e = e.InnerException)
			{
				if (e is DbException dbe && (dbe.SqlState == "40001" || dbe.SqlState == "40P01"))
					return true;
				if (e.Message.IndexOf( "deadlock", StringComparison.OrdinalIgnoreCase ) >= 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Creates a customer and an order with lines and two tags.
		/// </summary>
		async Task CreateAsync( IterationState s, CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				var customer = new Customer()
				{
					Name = s.CustomerName,
					City = "City" + this.workerNo,
					// Some databases don't store fractions of seconds
					Created = new DateTime( 2026, 1, 1, 12, 0, 0 ).AddMinutes( this.workerNo )
				};

				var order = new SalesOrder()
				{
					OrderNumber = s.OrderNumber,
					OrderDate = DateTime.Today,
					Status = "New",
					Customer = customer
				};

				for (int i = 0; i < this.options.LinesPerOrder; i++)
					order.NewLine( "Product" + i, i + 1, 1.25m * (i + 1) );
				order.Total = order.ComputeTotal();

				// FindObject creates hollow objects without database access
				var tagIndexes = PickDistinct( 2 );
				foreach (int index in tagIndexes)
					order.AddTag( (Tag) pm.FindObject( typeof( Tag ), this.tagIds[index] ) );

				pm.MakePersistent( customer );
				pm.MakePersistent( order );
				await this.statistics.MeasureAsync( "SaveAsync (insert)", () => pm.SaveAsync( false, ct ) );

				s.OrderId = IdOf( order );
				Check( s.OrderId > 0, $"Order {s.OrderNumber} didn't get an id." );
				Check( order.Lines.All( l => IdOf( l ) > 0 ), $"Not all lines of order {s.OrderNumber} got an id." );
				s.ExpectedTagIds = new HashSet<int>( tagIndexes.Select( i => this.tagIds[i] ) );
			}
		}

		/// <summary>
		/// Reads the order, changes fields, lines, tags and the customer and saves the changes.
		/// </summary>
		async Task ChangeAsync( IterationState s, CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				var order = await this.statistics.MeasureAsync( "VirtualTable.SingleAsync",
					() => pm.Objects<SalesOrder>().Where( o => o.OrderNumber == s.OrderNumber ).SingleAsync( ct ) );
				Check( IdOf( order ) == s.OrderId, $"Order {s.OrderNumber}: wrong id {IdOf( order )}, expected {s.OrderId}." );

				await this.statistics.MeasureAsync( "LoadRelationAsync", () => pm.LoadRelationAsync( order, "lines", false, ct ) );
				await this.statistics.MeasureAsync( "LoadRelationAsync", () => pm.LoadRelationAsync( order, "tags", false, ct ) );
				var lines = order.Lines.ToList();
				Check( lines.Count == this.options.LinesPerOrder, $"Order {s.OrderNumber}: {lines.Count} lines read, expected {this.options.LinesPerOrder}." );
				var tagIdsRead = new HashSet<int>( order.Tags.Select( IdOf ) );
				Check( tagIdsRead.SetEquals( s.ExpectedTagIds ), $"Order {s.OrderNumber}: wrong tags [{String.Join( ",", tagIdsRead )}], expected [{String.Join( ",", s.ExpectedTagIds )}]." );

				Customer customer = order.Customer;
				Check( customer != null, $"Order {s.OrderNumber} has no customer." );
				await this.statistics.MeasureAsync( "LoadDataAsync", () => pm.LoadDataAsync( customer, ct ) );
				Check( customer.Name == s.CustomerName, $"Order {s.OrderNumber}: wrong customer '{customer.Name}', expected '{s.CustomerName}'." );

				// Change fields, change a line, remove a line, add a line
				order.Status = "Changed";
				var first = lines.OrderBy( l => l.Product ).First();
				first.Quantity += 10;
				order.RemoveLine( lines.OrderBy( l => l.Product ).Last() );
				order.NewLine( "Extra", 7, 3.5m );
				order.Total = order.ComputeTotal();

				// Replace one tag by another one
				var removedTag = order.Tags.First();
				order.RemoveTag( removedTag );
				int newTagId = this.tagIds.First( id => !s.ExpectedTagIds.Contains( id ) );
				order.AddTag( (Tag) pm.FindObject( typeof( Tag ), newTagId ) );

				customer.UpdateCount++;

				await this.statistics.MeasureAsync( "SaveAsync (update)", () => pm.SaveAsync( false, ct ) );

				s.ExpectedTagIds.Remove( IdOf( removedTag ) );
				s.ExpectedTagIds.Add( newTagId );
				s.ExpectedLineCount = order.Lines.Count();
				s.ExpectedPriceSum = order.Lines.Sum( l => l.Price );
				s.ExpectedTotal = order.Total;
			}
		}

		/// <summary>
		/// Checks the changes with a new PersistenceManager.
		/// </summary>
		async Task VerifyAsync( IterationState s, CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				int orderId = s.OrderId;
				var lineQuery = pm.Objects<OrderLine>().Where( l => l.Order.Oid() == orderId );

				var lines = await this.statistics.MeasureAsync( "VirtualTable.ToListAsync", () => lineQuery.ToListAsync( ct ) );
				Check( lines.Count == s.ExpectedLineCount, $"Order {s.OrderNumber}: {lines.Count} lines found, expected {s.ExpectedLineCount}." );
				Check( lines.Any( l => l.Product == "Extra" ), $"Order {s.OrderNumber}: the added line is missing." );
				Check( !lines.Any( l => l.Product == "Product" + (this.options.LinesPerOrder - 1) ), $"Order {s.OrderNumber}: the removed line still exists." );

				int count = await this.statistics.MeasureAsync( "VirtualTable.CountAsync", () => lineQuery.CountAsync( ct ) );
				Check( count == s.ExpectedLineCount, $"Order {s.OrderNumber}: CountAsync returns {count}, expected {s.ExpectedLineCount}." );

				// Note: Sum over an int field doesn't work, because NDO always computes sums as decimal and casts the result to TP
				decimal priceSum = await this.statistics.MeasureAsync( "VirtualTable.SumAsync", () => lineQuery.SumAsync( l => l.Price, ct ) );
				Check( priceSum == s.ExpectedPriceSum, $"Order {s.OrderNumber}: SumAsync returns {priceSum}, expected {s.ExpectedPriceSum}." );

				// await foreach over a VirtualTable with a condition on a related object
				string customerName = s.CustomerName;
				var orders = await this.statistics.MeasureAsync( "VirtualTable await foreach", async () =>
				{
					var result = new List<SalesOrder>();
					await foreach (var o in pm.Objects<SalesOrder>().Where( o => o.Customer.Name == customerName ).AsAsyncEnumerable( ct ))
						result.Add( o );
					return result;
				} );
				Check( orders.Count == 1, $"Customer {customerName}: {orders.Count} orders found, expected 1." );

				var order = orders[0];
				Check( order.OrderNumber == s.OrderNumber, $"Customer {customerName}: wrong order {order.OrderNumber}, expected {s.OrderNumber}." );
				Check( order.Status == "Changed", $"Order {s.OrderNumber}: status '{order.Status}', expected 'Changed'." );
				Check( order.Total == s.ExpectedTotal, $"Order {s.OrderNumber}: total {order.Total}, expected {s.ExpectedTotal}." );
				decimal linesTotal = lines.Sum( l => l.Quantity * l.Price );
				Check( linesTotal == s.ExpectedTotal, $"Order {s.OrderNumber}: the lines sum up to {linesTotal}, expected {s.ExpectedTotal}." );

				await this.statistics.MeasureAsync( "LoadRelationAsync", () => pm.LoadRelationAsync( order, "tags", false, ct ) );
				var tagIdsRead = new HashSet<int>( order.Tags.Select( IdOf ) );
				Check( tagIdsRead.SetEquals( s.ExpectedTagIds ), $"Order {s.OrderNumber}: wrong tags [{String.Join( ",", tagIdsRead )}], expected [{String.Join( ",", s.ExpectedTagIds )}]." );

				var customer = await this.statistics.MeasureAsync( "VirtualTable.FirstOrDefaultAsync",
					() => pm.Objects<Customer>().Where( c => c.Name == customerName ).FirstOrDefaultAsync( ct ) );
				Check( customer != null, $"Customer {customerName} not found." );
				Check( customer.UpdateCount == 1, $"Customer {customerName}: UpdateCount is {customer.UpdateCount}, expected 1." );

				// RefreshAsync reads the object again
				await this.statistics.MeasureAsync( "RefreshAsync", () => pm.RefreshAsync( customer, ct ) );
				Check( customer.UpdateCount == 1, $"Customer {customerName}: UpdateCount after RefreshAsync is {customer.UpdateCount}, expected 1." );
			}
		}

		/// <summary>
		/// Queries, which read data of other workers. The results can't be checked, because the other workers change the data.
		/// </summary>
		async Task QueryAsync( CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				await this.statistics.MeasureAsync( "VirtualTable.CountAsync", () => pm.Objects<SalesOrder>().Where( o => o.Status == "Changed" ).CountAsync( ct ) );
				var page = await this.statistics.MeasureAsync( "VirtualTable paged",
					() => pm.Objects<SalesOrder>().OrderByDescending( o => o.OrderNumber ).Take( 10 ).ToListAsync( ct ) );
				Check( page.Count <= 10, $"Take(10) returned {page.Count} objects." );
				int tagCount = await this.statistics.MeasureAsync( "VirtualTable.CountAsync", () => pm.Objects<Tag>().CountAsync( ct ) );
				Check( tagCount == this.tagIds.Length, $"{tagCount} tags found, expected {this.tagIds.Length}." );
			}
		}

		/// <summary>
		/// Deletes the order with PersistenceManager.Delete (the composite relation deletes the lines and the mapping table entries).
		/// </summary>
		async Task DeleteOrderAsync( IterationState s, CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				var order = await this.statistics.MeasureAsync( "VirtualTable.SingleAsync",
					() => pm.Objects<SalesOrder>().Where( o => o.OrderNumber == s.OrderNumber ).SingleAsync( ct ) );
				pm.Delete( order );
				await this.statistics.MeasureAsync( "SaveAsync (delete)", () => pm.SaveAsync( false, ct ) );
			}
		}

		/// <summary>
		/// Deletes the customer with VirtualTable.DeleteDirectlyAsync.
		/// </summary>
		async Task DeleteCustomerAsync( IterationState s, CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				string customerName = s.CustomerName;
				await this.statistics.MeasureAsync( "VirtualTable.DeleteDirectlyAsync",
					() => pm.Objects<Customer>().Where( c => c.Name == customerName ).DeleteDirectlyAsync( ct ) );
			}
		}

		async Task VerifyDeletionAsync( IterationState s, CancellationToken ct )
		{
			await using (var pm = this.db.NewPersistenceManager())
			{
				string orderNumber = s.OrderNumber;
				string customerName = s.CustomerName;
				int orderId = s.OrderId;

				int orders = await pm.Objects<SalesOrder>().Where( o => o.OrderNumber == orderNumber ).CountAsync( ct );
				Check( orders == 0, $"Order {orderNumber} still exists after deletion." );
				int lines = await pm.Objects<OrderLine>().Where( l => l.Order.Oid() == orderId ).CountAsync( ct );
				Check( lines == 0, $"Order {orderNumber}: {lines} lines still exist after deletion." );
				int customers = await pm.Objects<Customer>().Where( c => c.Name == customerName ).CountAsync( ct );
				Check( customers == 0, $"Customer {customerName} still exists after DeleteDirectlyAsync." );
			}
		}

		int[] PickDistinct( int count )
		{
			return Enumerable.Range( 0, this.tagIds.Length ).OrderBy( _ => this.random.Next() ).Take( count ).ToArray();
		}
	}
}
